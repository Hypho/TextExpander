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
        try
        {
            ApplicationConfiguration.Initialize();

            var configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TextExpander", "rules.json");
            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TextExpander", "settings.json");

            var configManager = new ConfigManager(configPath, settingsPath);
            var rules = configManager.LoadRules();
            var appConfig = configManager.LoadAppConfig();
            var engine = new HookEngine(rules, appConfig: appConfig);
            var bootManager = new BootManager();

            var tray = new TrayIcon(engine, bootManager,
                () => new RuleManagerForm(configManager),
                () => new SettingsForm(configManager, bootManager, engine));

            if (!engine.Start())
            {
                MessageBox.Show(
                    "键盘钩子启动失败，请检查是否有安全软件拦截。\nTextExpander 将以暂停模式运行。",
                    "TextExpander",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            Application.Run();

            engine.Dispose();
            tray.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"启动异常：{ex.Message}", "TextExpander Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
