using TextExpander.Config;
using TextExpander.Core;
using TextExpander.Startup;

namespace TextExpander.UI;

public class SettingsForm : Form
{
    private readonly ConfigManager _configManager;
    private readonly BootManager _bootManager;
    private readonly HookEngine _engine;
    private readonly CheckBox _chkTab;
    private readonly CheckBox _chkSpace;
    private readonly CheckBox _chkEnter;
    private readonly CheckBox _chkStartOnBoot;

    public SettingsForm(ConfigManager configManager, BootManager bootManager, HookEngine engine)
    {
        _configManager = configManager;
        _bootManager = bootManager;
        _engine = engine;

        Text = "设置";
        Size = new Size(350, 220);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var lblTerminators = new Label { Text = "终止键：", Location = new Point(20, 25), AutoSize = true };
        _chkTab = new CheckBox { Text = "Tab", Location = new Point(100, 23), AutoSize = true };
        _chkSpace = new CheckBox { Text = "Space", Location = new Point(170, 23), AutoSize = true };
        _chkEnter = new CheckBox { Text = "Enter", Location = new Point(250, 23), AutoSize = true };

        _chkStartOnBoot = new CheckBox { Text = "开机自启", Location = new Point(100, 70), AutoSize = true };

        var btnSave = new Button { Text = "保存", Location = new Point(140, 130), Size = new Size(80, 30) };
        var btnCancel = new Button { Text = "取消", Location = new Point(230, 130), Size = new Size(80, 30) };

        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { lblTerminators, _chkTab, _chkSpace, _chkEnter, _chkStartOnBoot, btnSave, btnCancel });

        AcceptButton = btnSave;
        CancelButton = btnCancel;

        LoadCurrentConfig();
    }

    private void LoadCurrentConfig()
    {
        var config = _configManager.LoadAppConfig();
        if (_configManager.LastAppConfigLoadHadError)
        {
            MessageBox.Show("配置异常，已恢复默认", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        var keys = config.TerminatorKeys ?? new List<string>();
        _chkTab.Checked = keys.Contains("Tab");
        _chkSpace.Checked = keys.Contains("Space");
        _chkEnter.Checked = keys.Contains("Enter");
        _chkStartOnBoot.Checked = _bootManager.IsStartupEnabled();
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var keys = GetSelectedKeys();
        var validation = SettingsValidator.ValidateTerminatorKeys(keys);

        if (validation == SettingsValidationResult.NoTerminatorKeys)
        {
            MessageBox.Show("至少保留一个终止键", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var config = new AppConfig
        {
            TerminatorKeys = keys,
            Enabled = true,
            StartOnBoot = _chkStartOnBoot.Checked
        };
        _configManager.SaveAppConfig(config);
        _bootManager.SetStartup(_chkStartOnBoot.Checked);

        // Apply terminator keys immediately
        _engine.ReloadTerminators(keys);

        DialogResult = DialogResult.OK;
        Close();
    }

    private List<string> GetSelectedKeys()
    {
        var keys = new List<string>();
        if (_chkTab.Checked) keys.Add("Tab");
        if (_chkSpace.Checked) keys.Add("Space");
        if (_chkEnter.Checked) keys.Add("Enter");
        return keys;
    }
}
