using TextExpander.Config;
using TextExpander.Core;
using TextExpander.Startup;

namespace TextExpander.Tests.UI;

public class TrayIconTests
{
    private static List<Rule> DefaultRules() => new()
    {
        new() { Id = "1", Abbreviation = ";test", Replacement = "hello", Enabled = true }
    };

    [Fact]
    [Trait("Category", "Smoke")]
    public void Engine_Toggle_ChangesState()
    {
        var rules = DefaultRules();
        using var engine = new HookEngine(rules, new FakeTextSender());

        Assert.Equal(EngineState.Active, engine.State);
        engine.Toggle();
        Assert.Equal(EngineState.Paused, engine.State);
        engine.Toggle();
        Assert.Equal(EngineState.Active, engine.State);
    }

    [Fact]
    public void BootManager_ToggleStartup_ChangesState()
    {
        var manager = new BootManager();
        var initial = manager.IsStartupEnabled();
        manager.ToggleStartup();
        Assert.NotEqual(initial, manager.IsStartupEnabled());
        manager.ToggleStartup();
        Assert.Equal(initial, manager.IsStartupEnabled());
    }

    [Fact]
    public void Engine_State_AffectsMenuText()
    {
        var rules = DefaultRules();
        using var engine = new HookEngine(rules, new FakeTextSender());

        // Simulate menu text logic
        var menuText = engine.State == EngineState.Active ? "暂停" : "恢复";
        Assert.Equal("暂停", menuText);

        engine.Toggle();
        menuText = engine.State == EngineState.Active ? "暂停" : "恢复";
        Assert.Equal("恢复", menuText);
    }

    [Fact]
    public void StartupItem_Checked_ReflectsBootManagerState()
    {
        var manager = new BootManager();
        manager.SetStartup(false);
        Assert.False(manager.IsStartupEnabled());

        manager.SetStartup(true);
        Assert.True(manager.IsStartupEnabled());

        // Restore
        manager.SetStartup(false);
    }

    private class FakeTextSender : ITextSender
    {
        public void SendBackspaces(int count) { }
        public void SendText(string text) { }
    }
}
