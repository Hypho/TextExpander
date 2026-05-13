using System.Text.Json;

namespace TextExpander.Config;

public class ConfigManager
{
    private readonly string _configPath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public ConfigManager(string configPath)
    {
        _configPath = configPath;
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
}
