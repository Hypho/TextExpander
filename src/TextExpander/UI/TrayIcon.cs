using TextExpander.Core;
using TextExpander.Startup;

namespace TextExpander.UI;

public class TrayIcon : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly HookEngine _engine;
    private readonly BootManager _bootManager;
    private readonly Func<RuleManagerForm> _formFactory;
    private readonly Func<SettingsForm> _settingsFactory;
    private RuleManagerForm? _managerForm;
    private SettingsForm? _settingsForm;
    private readonly ToolStripMenuItem _pauseResumeItem;
    private readonly ToolStripMenuItem _startupItem;

    public TrayIcon(HookEngine engine, BootManager bootManager, Func<RuleManagerForm> formFactory, Func<SettingsForm> settingsFactory)
    {
        _engine = engine;
        _bootManager = bootManager;
        _formFactory = formFactory;
        _settingsFactory = settingsFactory;

        _pauseResumeItem = new ToolStripMenuItem("暂停");
        _pauseResumeItem.Click += (s, e) => TogglePause();

        var manageItem = new ToolStripMenuItem("管理规则");
        manageItem.Click += (s, e) => ShowManagerForm();

        var settingsItem = new ToolStripMenuItem("设置");
        settingsItem.Click += (s, e) => ShowSettingsForm();

        _startupItem = new ToolStripMenuItem("开机自启");
        _startupItem.Click += (s, e) => ToggleStartup();

        var exitItem = new ToolStripMenuItem("退出");
        exitItem.Click += (s, e) => ExitApp();

        var menu = new ContextMenuStrip();
        menu.Items.AddRange(new ToolStripItem[] { _pauseResumeItem, manageItem, settingsItem, _startupItem, new ToolStripSeparator(), exitItem });

        _notifyIcon = new NotifyIcon
        {
            Icon = CreateDefaultIcon(),
            Text = "TextExpander",
            ContextMenuStrip = menu,
            Visible = true
        };

        _engine.OnStateChanged += OnEngineStateChanged;
        UpdateMenuState();
    }

    private void OnEngineStateChanged(EngineState state)
    {
        UpdateMenuState();
    }

    private void TogglePause()
    {
        _engine.Toggle();
        UpdateMenuState();
    }

    private void ShowManagerForm()
    {
        if (_managerForm == null || _managerForm.IsDisposed)
        {
            _managerForm = _formFactory();
            _managerForm.FormClosed += (s, e) => { _managerForm = null; };
            _managerForm.Show();
        }
        else
        {
            _managerForm.Activate();
            _managerForm.WindowState = FormWindowState.Normal;
        }
    }

    private void ShowSettingsForm()
    {
        if (_settingsForm == null || _settingsForm.IsDisposed)
        {
            _settingsForm = _settingsFactory();
            _settingsForm.FormClosed += (s, e) => { _settingsForm = null; };
            _settingsForm.Show();
        }
        else
        {
            _settingsForm.Activate();
            _settingsForm.WindowState = FormWindowState.Normal;
        }
    }

    private void ToggleStartup()
    {
        if (!_bootManager.SetStartup(!_bootManager.IsStartupEnabled()))
        {
            MessageBox.Show("设置开机自启失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        UpdateMenuState();
    }

    private void ExitApp()
    {
        Dispose();
        Application.Exit();
    }

    private void UpdateMenuState()
    {
        _pauseResumeItem.Text = _engine.State == EngineState.Active ? "暂停" : "恢复";
        _startupItem.Checked = _bootManager.IsStartupEnabled();
    }

    private static Icon CreateDefaultIcon()
    {
        var bmp = new Bitmap(16, 16);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Transparent);
        g.FillEllipse(Brushes.DodgerBlue, 1, 1, 14, 14);
        g.DrawString("T", new Font("Arial", 9, FontStyle.Bold), Brushes.White, 2, 1);
        return Icon.FromHandle(bmp.GetHicon());
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _managerForm?.Dispose();
    }
}
