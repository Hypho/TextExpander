using TextExpander.Config;
using TextExpander.UI;

namespace TextExpander;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TextExpander", "rules.json");

        var configManager = new ConfigManager(configPath);
        var form = new RuleManagerForm(configManager);
        Application.Run(form);
    }
}
