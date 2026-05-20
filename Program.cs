using TextExpander.Config;
using TextExpander.Engine;
using TextExpander.Hook;
using TextExpander.UI;

namespace TextExpander;

static class Program
{
    private const uint TriggerKey = 0x09; // Tab

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();

        var store = new RuleStore();
        store.Load();

        var buffer = new InputBuffer();
        var matcher = new RuleMatcher(store);
        var executor = new TextCommandExecutor();

        using var hook = new KeyboardHook();

        hook.KeyDown += vk =>
        {
            if (vk == 0x08) { buffer.PopBack(); return; }

            char? c = VkToChar(vk);
            if (c.HasValue) buffer.Push(c.Value);

            if (vk == TriggerKey)
            {
                string? proc = ProcessHelper.GetForegroundProcessName();
                var result = matcher.TryMatch(buffer.Current, proc);

                if (result.HasValue)
                {
                    var (rule, captured) = result.Value;
                    buffer.Reset();

                    Task.Run(() =>
                    {
                        Thread.Sleep(20);
                        executor.SendBackspaceForTriggerKey();
                        executor.Erase(rule, captured);
                        executor.Output(rule.Output);
                    });
                    return;
                }

                buffer.Push('\t');
            }
        };

        Application.Run(new TrayApp(store));
    }

    private static char? VkToChar(uint vk)
    {
        if (vk >= 0x30 && vk <= 0x39) return (char)('0' + (vk - 0x30));
        if (vk >= 0x41 && vk <= 0x5A) return (char)('a' + (vk - 0x41));
        return null;
    }
}
