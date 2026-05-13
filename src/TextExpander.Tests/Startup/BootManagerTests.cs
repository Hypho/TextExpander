using TextExpander.Startup;

namespace TextExpander.Tests.Startup;

public class BootManagerTests
{
    [Fact]
    [Trait("Category", "Smoke")]
    public void IsStartupEnabled_ReturnsBool()
    {
        var manager = new BootManager();
        var result = manager.IsStartupEnabled();
        // Just verify it doesn't throw and returns a bool
        Assert.IsType<bool>(result);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void SetStartup_Enable_Succeeds()
    {
        var manager = new BootManager();
        var result = manager.SetStartup(true);
        Assert.True(result);
        Assert.True(manager.IsStartupEnabled());
    }

    [Fact]
    public void SetStartup_Disable_Succeeds()
    {
        var manager = new BootManager();
        manager.SetStartup(true); // ensure enabled first
        var result = manager.SetStartup(false);
        Assert.True(result);
        Assert.False(manager.IsStartupEnabled());
    }

    [Fact]
    public void ToggleStartup_TogglesState()
    {
        var manager = new BootManager();
        var initial = manager.IsStartupEnabled();
        manager.ToggleStartup();
        Assert.NotEqual(initial, manager.IsStartupEnabled());
        // Restore
        manager.ToggleStartup();
        Assert.Equal(initial, manager.IsStartupEnabled());
    }
}
