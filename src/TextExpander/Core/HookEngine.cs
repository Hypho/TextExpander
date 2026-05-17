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

    public EngineState State { get; private set; } = EngineState.Active;
    public bool IsHookInstalled => _hook.IsInstalled;

    public event Action<string>? OnNotification;

    public HookEngine(List<Rule> rules, ITextSender? textSender = null, Func<string>? clipboardProvider = null)
    {
        _resolver = new VariableResolver(clipboardProvider);
        var sender = textSender ?? new WinTextSender();
        _inputBuffer = new InputBuffer(maxLength: 200);
        _matcher = new AbbreviationMatcher(rules);
        _replacer = new TextReplacer(sender, _resolver);

        _hook = new KeyboardHook(
            onCharTyped: OnCharTyped,
            onTerminatorKey: OnTerminatorKey,
            onBackspace: OnBackspace,
            shouldSuppress: ShouldSuppress
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

    public void ReloadRules(List<Rule> rules)
    {
        _matcher.ReloadRules(rules);
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
