using System.Text.Json;

namespace TextExpander.Config;

public class RuleStore
{
    private static readonly string ConfigDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TextExpander");
    private static readonly string ConfigPath = Path.Combine(ConfigDir, "rules.json");

    public List<TextRule> Rules { get; private set; } = new();

    public void Load()
    {
        if (!File.Exists(ConfigPath)) return;
        var json = File.ReadAllText(ConfigPath);
        Rules = JsonSerializer.Deserialize<List<TextRule>>(json) ?? new();
    }

    public void Save()
    {
        Directory.CreateDirectory(ConfigDir);
        File.WriteAllText(ConfigPath,
            JsonSerializer.Serialize(Rules, new JsonSerializerOptions { WriteIndented = true }));
    }
}
