using TextExpander.Config;

namespace TextExpander.UI;

public class RuleEditDialog : Form
{
    private readonly TextBox _txtAbbreviation;
    private readonly TextBox _txtReplacement;
    private readonly CheckBox _chkEnabled;
    private readonly List<Rule> _existingRules;
    private readonly string? _editingRuleId;

    public Rule? Result { get; private set; }

    public RuleEditDialog(List<Rule> existingRules, Rule? ruleToEdit = null)
    {
        _existingRules = existingRules;
        _editingRuleId = ruleToEdit?.Id;

        Text = ruleToEdit == null ? "添加规则" : "编辑规则";
        Size = new Size(450, 250);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        var lblAbbr = new Label { Text = "缩写词：", Location = new Point(20, 25), AutoSize = true };
        _txtAbbreviation = new TextBox { Location = new Point(100, 22), Size = new Size(300, 23) };

        var lblRepl = new Label { Text = "替换文本：", Location = new Point(20, 65), AutoSize = true };
        _txtReplacement = new TextBox { Location = new Point(100, 62), Size = new Size(300, 60), Multiline = true };

        _chkEnabled = new CheckBox { Text = "启用", Location = new Point(100, 135), Checked = true };

        var btnSave = new Button { Text = "保存", Location = new Point(230, 170), Size = new Size(80, 30) };
        var btnCancel = new Button { Text = "取消", Location = new Point(320, 170), Size = new Size(80, 30) };

        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { lblAbbr, _txtAbbreviation, lblRepl, _txtReplacement, _chkEnabled, btnSave, btnCancel });

        if (ruleToEdit != null)
        {
            _txtAbbreviation.Text = ruleToEdit.Abbreviation;
            _txtReplacement.Text = ruleToEdit.Replacement;
            _chkEnabled.Checked = ruleToEdit.Enabled;
        }

        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var validation = RuleValidator.Validate(_txtAbbreviation.Text, _txtReplacement.Text, _existingRules, _editingRuleId);

        switch (validation)
        {
            case ValidationResult.EmptyAbbreviation:
                MessageBox.Show("缩写词和替换文本不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            case ValidationResult.EmptyReplacement:
                MessageBox.Show("缩写词和替换文本不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            case ValidationResult.DuplicateAbbreviation:
                MessageBox.Show("该缩写词已存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
        }

        Result = new Rule
        {
            Id = _editingRuleId ?? Guid.NewGuid().ToString(),
            Abbreviation = _txtAbbreviation.Text.Trim(),
            Replacement = _txtReplacement.Text,
            Enabled = _chkEnabled.Checked,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            UpdatedAt = _editingRuleId != null ? DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") : null
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}
