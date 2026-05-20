using TextExpander.Config;

namespace TextExpander.Core;

public enum EngineState
{
    Active,
    Paused
}

public class HookEngine : IDisposable
{
    private readonly InputBuffer _inputBuffer;
    private readonly AbbreviationMatcher _matcher;
    private readonly TextReplacer _replacer;
    private readonly KeyboardHook _hook;
    private readonly VariableResolver _resolver;
    private bool _suppressCurrentKey;
    private HashSet<int> _terminatorVkCodes;
    private EngineState _state = EngineState.Active;

    public EngineState State
    {
        get => _state;
        private set
        {
            _state = value;
            OnStateChanged?.Invoke(_state);
        }
    }
    public bool IsHookInstalled => _hook.IsInstalled;
    public IReadOnlySet<int> CurrentTerminatorVkCodes => _terminatorVkCodes;

    public event Action<string>? OnNotification;
    public event Action<EngineState>? OnStateChanged;

    public HookEngine(List<Rule> rules, ITextSender? textSender = null, Func<string>? clipboardProvider = null, AppConfig? appConfig = null)
    {
        _resolver = new VariableResolver(clipboardProvider);
        var sender = textSender ?? new WinTextSender();
        _inputBuffer = new InputBuffer(maxLength: 200);
        _matcher = new AbbreviationMatcher(rules);
        _replacer = new TextReplacer(sender, _resolver);

        var config = appConfig ?? new AppConfig();
        State = config.Enabled ? EngineState.Active : EngineState.Paused;
        _terminatorVkCodes = BuildTerminatorSet(config.TerminatorKeys);

        _hook = new KeyboardHook(
            onCharTyped: OnCharTyped,
            onTerminatorKey: OnTerminatorKey,
            onBackspace: OnBackspace,
            shouldSuppress: ShouldSuppress,
            terminatorVkCodes: _terminatorVkCodes
        );
    }

    public bool Start()
    {
        if (!_hook.Install())
        {
            OnNotification?.Invoke("键盘钩子启动失败");
            return false;
        }
        return true;
    }

    public void Stop()
    {
        _hook.Uninstall();
    }

    public void Toggle()
    {
        if (State == EngineState.Active)
        {
            State = EngineState.Paused;
            Stop();
        }
        else
        {
            State = EngineState.Active;
            Start();
        }
    }

    public void SetEnabled(bool enabled)
    {
        State = enabled ? EngineState.Active : EngineState.Paused;
        if (enabled && !IsHookInstalled)
            Start();
        else if (!enabled && IsHookInstalled)
            Stop();
    }

    public void ReloadRules(List<Rule> rules)
    {
        _matcher.ReloadRules(rules);
    }

    public void ReloadTerminators(List<string> keys)
    {
        _terminatorVkCodes = BuildTerminatorSet(keys);
        _hook.UpdateTerminators(_terminatorVkCodes);
        OnNotification?.Invoke($"终止键已更新: {string.Join(", ", keys)}");
    }

    private static HashSet<int> BuildTerminatorSet(List<string>? keys)
    {
        if (keys == null || keys.Count == 0)
            return new HashSet<int> { 0x09, 0x20, 0x0D };

        var set = new HashSet<int>();
        foreach (var key in keys)
        {
            switch (key)
            {
                case "Tab": set.Add(0x09); break;
                case "Space": set.Add(0x20); break;
                case "Enter": set.Add(0x0D); break;
            }
        }
        return set.Count > 0 ? set : new HashSet<int> { 0x09, 0x20, 0x0D };
    }

    private void OnCharTyped(char c)
    {
        if (State != EngineState.Active) return;
        _inputBuffer.Append(c);
    }

    private void OnBackspace()
    {
        if (State != EngineState.Active) return;
        _inputBuffer.Backspace();
    }

    private void OnTerminatorKey(int vkCode)
    {
        if (State != EngineState.Active) return;

        var content = _inputBuffer.Content;
        if (string.IsNullOrEmpty(content))
            return;

        var rule = _matcher.TryMatch(content);
        if (rule != null)
        {
            _suppressCurrentKey = true;
            _replacer.Replace(rule.Abbreviation, rule.Replacement);
            _inputBuffer.Clear();
        }
        else
        {
            _inputBuffer.Clear();
        }
    }

    private bool ShouldSuppress(int vkCode)
    {
        if (_suppressCurrentKey)
        {
            _suppressCurrentKey = false;
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        _hook.Dispose();
    }
}
