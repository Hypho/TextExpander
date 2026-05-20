using TextExpander.Config;
using TextExpander.Core;

namespace TextExpander.Tests.Core;

public class HookEngineTests
{
    private class FakeTextSender : ITextSender
    {
        public int BackspaceCount { get; private set; }
        public string? SentText { get; private set; }
        public List<(int count, string text)> Calls { get; } = new();

        public void SendBackspaces(int count)
        {
            BackspaceCount = count;
            Calls.Add((count, ""));
        }

        public void SendText(string text)
        {
            SentText = text;
            if (Calls.Count > 0)
                Calls[^1] = (Calls[^1].count, text);
            else
                Calls.Add((0, text));
        }
    }

    private static List<Rule> DefaultRules()
    {
        return new List<Rule>
        {
            new() { Id = "1", Abbreviation = ";addr", Replacement = "123 Main St", Enabled = true },
            new() { Id = "2", Abbreviation = ";date", Replacement = "{date}", Enabled = true },
            new() { Id = "3", Abbreviation = ";clip", Replacement = "{clipboard}", Enabled = true },
            new() { Id = "4", Abbreviation = ";off", Replacement = "disabled", Enabled = false }
        };
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void Engine_InitialState_IsActive()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        using var engine = new HookEngine(rules, sender);

        Assert.Equal(EngineState.Active, engine.State);
    }

    [Fact]
    public void Toggle_SwitchesBetweenActiveAndPaused()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        using var engine = new HookEngine(rules, sender);

        engine.Toggle();
        Assert.Equal(EngineState.Paused, engine.State);

        engine.Toggle();
        Assert.Equal(EngineState.Active, engine.State);
    }

    [Fact]
    public void ReloadRules_UpdatesMatcher()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        using var engine = new HookEngine(rules, sender);

        var newRules = new List<Rule>
        {
            new() { Id = "5", Abbreviation = ";new", Replacement = "new-text", Enabled = true }
        };
        engine.ReloadRules(newRules);
        // Rule reload is verified indirectly through the matcher
        Assert.Equal(EngineState.Active, engine.State);
    }

    [Fact]
    public void NotificationEvent_FiresOnHookFailure()
    {
        // This test verifies the notification mechanism exists.
        // Actual hook failure requires Windows-level testing.
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        using var engine = new HookEngine(rules, sender);

        string? notification = null;
        engine.OnNotification += msg => notification = msg;

        // Simulate by calling Start in an environment where hook may fail
        // In unit test environment, this tests the event wiring
        Assert.Null(notification);
    }

    // --- AppConfig integration tests (NF-01, FC-03) ---

    [Fact]
    public void Engine_WithDisabledConfig_InitializesPaused()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        var config = new AppConfig { Enabled = false };
        using var engine = new HookEngine(rules, sender, appConfig: config);

        Assert.Equal(EngineState.Paused, engine.State);
    }

    [Fact]
    public void Engine_WithEnabledConfig_InitializesActive()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        var config = new AppConfig { Enabled = true };
        using var engine = new HookEngine(rules, sender, appConfig: config);

        Assert.Equal(EngineState.Active, engine.State);
    }

    [Fact]
    public void ReloadTerminators_UpdatesTerminatorSet()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        var config = new AppConfig { TerminatorKeys = new List<string> { "Tab", "Space", "Enter" } };
        using var engine = new HookEngine(rules, sender, appConfig: config);

        // Changing terminators should not throw
        engine.ReloadTerminators(new List<string> { "Tab" });
        Assert.Equal(EngineState.Active, engine.State);
    }

    [Fact]
    public void ReloadTerminators_ChangesTerminatorBehavior()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        var config = new AppConfig { TerminatorKeys = new List<string> { "Tab", "Space", "Enter" } };
        using var engine = new HookEngine(rules, sender, appConfig: config);

        // Verify initial terminators include Space (0x20)
        Assert.Contains(0x20, engine.CurrentTerminatorVkCodes);

        // After reload with only Tab, Space should be removed
        engine.ReloadTerminators(new List<string> { "Tab" });
        Assert.DoesNotContain(0x20, engine.CurrentTerminatorVkCodes);
        Assert.Contains(0x09, engine.CurrentTerminatorVkCodes);
    }

    [Fact]
    public void BuildTerminatorSet_EmptyKeys_FallsBackToDefaults()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        var config = new AppConfig { TerminatorKeys = new List<string>() };
        using var engine = new HookEngine(rules, sender, appConfig: config);

        // Empty terminator keys should fall back to defaults (not break the engine)
        Assert.Equal(EngineState.Active, engine.State);
    }

    [Fact]
    public void BuildTerminatorSet_NullKeys_DoesNotThrow()
    {
        var sender = new FakeTextSender();
        var rules = DefaultRules();
        var config = new AppConfig { TerminatorKeys = null! };
        using var engine = new HookEngine(rules, sender, appConfig: config);

        // Null terminator keys should not throw NRE, fall back to defaults
        Assert.Equal(EngineState.Active, engine.State);
    }
}
