using System.Diagnostics;

namespace TextExpander.UI;

public class TrayApp : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly Config.RuleStore _store;

    public TrayApp(Config.RuleStore store)
    {
        _store = store;

        _trayIcon = new NotifyIcon
        {
            Icon = CreateIcon(),
            Visible = true,
            Text = "TextExpander",
            ContextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem("编辑规则(&E)", null, (_, _) => EditRules()),
                    new ToolStripMenuItem("重新加载(&R)", null, (_, _) => ReloadRules()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("退出(&X)", null, (_, _) => Application.Exit())
                }
            }
        };
        _trayIcon.DoubleClick += (_, _) => EditRules();
    }

    private void EditRules()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TextExpander", "rules.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        if (!File.Exists(path))
            File.WriteAllText(path, "[]");
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    private void ReloadRules()
    {
        _store.Load();
        _trayIcon.ShowBalloonTip(2000, "TextExpander", "规则已重新加载", ToolTipIcon.Info);
    }

    private static System.Drawing.Icon CreateIcon()
    {
        using var bmp = new System.Drawing.Bitmap(16, 16);
        using var g = System.Drawing.Graphics.FromImage(bmp);
        g.Clear(System.Drawing.Color.Transparent);
        g.DrawString("T", new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
            System.Drawing.Brushes.DodgerBlue, 0, -1);
        return System.Drawing.Icon.FromHandle(bmp.GetHicon());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        }
        base.Dispose(disposing);
    }
}
