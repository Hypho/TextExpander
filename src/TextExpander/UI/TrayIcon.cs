using TextExpander.Core;
using TextExpander.Startup;

namespace TextExpander.UI;

public class TrayIcon : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly HookEngine _engine;
    private readonly BootManager _bootManager;
    private readonly Func<RuleManagerForm> _formFactory;
    private RuleManagerForm? _managerForm;
    private readonly ToolStripMenuItem _pauseResumeItem;
    private readonly ToolStripMenuItem _startupItem;

    public TrayIcon(HookEngine engine, BootManager bootManager, Func<RuleManagerForm> formFactory)
    {
        _engine = engine;
        _bootManager = bootManager;
        _formFactory = formFactory;

        _pauseResumeItem = new ToolStripMenuItem("暂停");
        _pauseResumeItem.Click += (s, e) => TogglePause();

        var manageItem = new ToolStripMenuItem("管理规则");
        manageItem.Click += (s, e) => ShowManagerForm();

        _startupItem = new ToolStripMenuItem("开机自启");
        _startupItem.Click += (s, e) => ToggleStartup();

        var exitItem = new ToolStripMenuItem("退出");
        exitItem.Click += (s, e) => ExitApp();

        var menu = new ContextMenuStrip();
        menu.Items.AddRange(new ToolStripItem[] { _pauseResumeItem, manageItem, _startupItem, new ToolStripSeparator(), exitItem });

        _notifyIcon = new NotifyIcon
        {
            Text = "TextExpander",
            ContextMenuStrip = menu,
            Visible = true
        };

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

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _managerForm?.Dispose();
    }
}
