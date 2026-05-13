using TextExpander.Config;
using TextExpander.UI;

namespace TextExpander.Tests.UI;

public class RuleManagerFormTests : IDisposable
{
    private readonly string _testDir;

    public RuleManagerFormTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"textexpander_gui_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void LoadRules_EmptyFile_ShowsZeroRules()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        var manager = new ConfigManager(configPath);

        // Simulate form load logic
        var rules = manager.LoadRules();
        Assert.Empty(rules);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void LoadRules_WithRules_LoadsAll()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        var manager = new ConfigManager(configPath);

        var rulesToSave = new List<Rule>
        {
            new() { Id = "1", Abbreviation = ";a", Replacement = "alpha", Enabled = true },
            new() { Id = "2", Abbreviation = ";b", Replacement = "beta", Enabled = false }
        };
        manager.SaveRules(rulesToSave);

        var rules = manager.LoadRules();
        Assert.Equal(2, rules.Count);
        Assert.Equal(";a", rules[0].Abbreviation);
        Assert.Equal(";b", rules[1].Abbreviation);
    }

    [Fact]
    public void AddRule_SavesToConfig_PersistsOnReload()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        var manager = new ConfigManager(configPath);

        var rules = manager.LoadRules();
        rules.Add(new Rule { Id = "new", Abbreviation = ";test", Replacement = "hello", Enabled = true });
        manager.SaveRules(rules);

        var reloaded = manager.LoadRules();
        Assert.Single(reloaded);
        Assert.Equal(";test", reloaded[0].Abbreviation);
    }

    [Fact]
    public void DeleteRule_RemovesFromConfig_PersistsOnReload()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        var manager = new ConfigManager(configPath);

        var rules = new List<Rule>
        {
            new() { Id = "1", Abbreviation = ";a", Replacement = "alpha", Enabled = true },
            new() { Id = "2", Abbreviation = ";b", Replacement = "beta", Enabled = true }
        };
        manager.SaveRules(rules);

        var loaded = manager.LoadRules();
        loaded.RemoveAt(0);
        manager.SaveRules(loaded);

        var reloaded = manager.LoadRules();
        Assert.Single(reloaded);
        Assert.Equal(";b", reloaded[0].Abbreviation);
    }

    [Fact]
    public void EditRule_UpdatesInConfig_PersistsOnReload()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        var manager = new ConfigManager(configPath);

        var rules = new List<Rule>
        {
            new() { Id = "1", Abbreviation = ";a", Replacement = "old", Enabled = true }
        };
        manager.SaveRules(rules);

        var loaded = manager.LoadRules();
        loaded[0].Replacement = "new";
        loaded[0].Enabled = false;
        manager.SaveRules(loaded);

        var reloaded = manager.LoadRules();
        Assert.Single(reloaded);
        Assert.Equal("new", reloaded[0].Replacement);
        Assert.False(reloaded[0].Enabled);
    }

    [Fact]
    public void LoadRules_CorruptJson_ReturnsEmptyList()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        File.WriteAllText(configPath, "not json!");
        var manager = new ConfigManager(configPath);

        var rules = manager.LoadRules();
        Assert.Empty(rules);
    }
}
