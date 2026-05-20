using System.Text.Json;

namespace TextExpander.Config;

public class ConfigManager
{
    private readonly string _configPath;
    private readonly string _settingsPath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public ConfigManager(string configPath, string? settingsPath = null)
    {
        _configPath = configPath;
        _settingsPath = settingsPath ?? Path.Combine(
            Path.GetDirectoryName(configPath) ?? ".",
            "settings.json");
    }

    public List<Rule> LoadRules()
    {
        if (!File.Exists(_configPath))
            return new List<Rule>();

        try
        {
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<List<Rule>>(json, JsonOptions) ?? new List<Rule>();
        }
        catch (JsonException)
        {
            return new List<Rule>();
        }
    }

    public void SaveRules(List<Rule> rules)
    {
        var dir = Path.GetDirectoryName(_configPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(rules, JsonOptions);
        File.WriteAllText(_configPath, json);
    }

    public bool LastAppConfigLoadHadError { get; private set; }

    public AppConfig LoadAppConfig()
    {
        LastAppConfigLoadHadError = false;
        if (!File.Exists(_settingsPath))
            return new AppConfig();

        try
        {
            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppConfig>(json, JsonOptions) ?? new AppConfig();
        }
        catch (JsonException)
        {
            LastAppConfigLoadHadError = true;
            return new AppConfig();
        }
    }

    public void SaveAppConfig(AppConfig config)
    {
        var dir = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(_settingsPath, json);
    }
}
