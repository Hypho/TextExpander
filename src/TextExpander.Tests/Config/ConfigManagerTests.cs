using TextExpander.Config;

namespace TextExpander.Tests.Config;

public class ConfigManagerTests : IDisposable
{
    private readonly string _testDir;

    public ConfigManagerTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"textexpander_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void LoadRules_FileDoesNotExist_ReturnsEmptyList()
    {
        var configPath = Path.Combine(_testDir, "nonexistent_rules.json");
        var manager = new ConfigManager(configPath);

        var rules = manager.LoadRules();

        Assert.Empty(rules);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void LoadRules_InvalidJson_ReturnsEmptyList()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        File.WriteAllText(configPath, "{ invalid json !!!");
        var manager = new ConfigManager(configPath);

        var rules = manager.LoadRules();

        Assert.Empty(rules);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void LoadRules_ValidJson_ReturnsRules()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        File.WriteAllText(configPath, """
        [
          {
            "id": "test-1",
            "abbreviation": ";addr",
            "replacement": "123 Main St",
            "enabled": true,
            "createdAt": "2026-01-01T00:00:00"
          }
        ]
        """);
        var manager = new ConfigManager(configPath);

        var rules = manager.LoadRules();

        Assert.Single(rules);
        Assert.Equal(";addr", rules[0].Abbreviation);
        Assert.Equal("123 Main St", rules[0].Replacement);
    }

    [Fact]
    public void LoadRules_EmptyJsonArray_ReturnsEmptyList()
    {
        var configPath = Path.Combine(_testDir, "rules.json");
        File.WriteAllText(configPath, "[]");
        var manager = new ConfigManager(configPath);

        var rules = manager.LoadRules();

        Assert.Empty(rules);
    }

    [Fact]
    public void SaveRules_CreatesFile_WhenNotExists()
    {
        var configPath = Path.Combine(_testDir, "new_rules.json");
        var manager = new ConfigManager(configPath);
        var rules = new List<Rule>
        {
            new() { Id = "1", Abbreviation = ";test", Replacement = "hello", Enabled = true }
        };

        manager.SaveRules(rules);

        Assert.True(File.Exists(configPath));
        var loaded = manager.LoadRules();
        Assert.Single(loaded);
        Assert.Equal(";test", loaded[0].Abbreviation);
    }
}
