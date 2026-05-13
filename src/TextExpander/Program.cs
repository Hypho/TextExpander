using TextExpander.Config;
using TextExpander.Core;
using TextExpander.Startup;
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
        var rules = configManager.LoadRules();
        var engine = new HookEngine(rules);
        var bootManager = new BootManager();

        // Tray icon (FC-01: app starts with tray icon, hook active)
        var tray = new TrayIcon(engine, bootManager, () => new RuleManagerForm(configManager));

        // Start keyboard hook (FC-01)
        engine.Start();

        // Run without main form — app lives in tray
        Application.Run();

        // Cleanup
        engine.Dispose();
        tray.Dispose();
    }
}
