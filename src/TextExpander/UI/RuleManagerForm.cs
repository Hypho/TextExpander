using TextExpander.Config;

namespace TextExpander.UI;

public class RuleManagerForm : Form
{
    private readonly ConfigManager _configManager;
    private readonly DataGridView _grid;
    private readonly Label _lblEmpty;
    private List<Rule> _rules;

    public RuleManagerForm(ConfigManager configManager)
    {
        _configManager = configManager;

        Text = "TextExpander — 规则管理";
        Size = new Size(650, 450);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(500, 350);

        // Toolbar
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(5) };

        var btnAdd = new Button { Text = "添加", Size = new Size(70, 30) };
        var btnEdit = new Button { Text = "编辑", Size = new Size(70, 30) };
        var btnDelete = new Button { Text = "删除", Size = new Size(70, 30) };

        btnAdd.Click += BtnAdd_Click;
        btnEdit.Click += BtnEdit_Click;
        btnDelete.Click += BtnDelete_Click;

        toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

        // Grid
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Abbreviation", HeaderText = "缩写词", FillWeight = 30 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Replacement", HeaderText = "替换文本", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Enabled", HeaderText = "启用", FillWeight = 20 });

        _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) EditSelectedRule(); };

        // Empty state label
        _lblEmpty = new Label
        {
            Text = "点击添加按钮创建第一条规则",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Font.FontFamily, 12),
            ForeColor = Color.Gray,
            Visible = false
        };

        Controls.Add(_grid);
        Controls.Add(_lblEmpty);
        Controls.Add(toolbar);

        LoadRules();
    }

    public void LoadRules()
    {
        _rules = _configManager.LoadRules();
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        _grid.Rows.Clear();
        foreach (var rule in _rules)
        {
            _grid.Rows.Add(rule.Abbreviation, rule.Replacement, rule.Enabled);
        }

        var isEmpty = _rules.Count == 0;
        _lblEmpty.Visible = isEmpty;
        _grid.Visible = !isEmpty;
    }

    private void SaveRules()
    {
        _configManager.SaveRules(_rules);
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dialog = new RuleEditDialog(_rules);
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Result != null)
        {
            _rules.Add(dialog.Result);
            SaveRules();
            RefreshGrid();
        }
    }

    private void EditSelectedRule()
    {
        if (_grid.SelectedRows.Count == 0) return;

        var index = _grid.SelectedRows[0].Index;
        var rule = _rules[index];

        using var dialog = new RuleEditDialog(_rules, rule);
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Result != null)
        {
            _rules[index] = dialog.Result;
            SaveRules();
            RefreshGrid();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e) => EditSelectedRule();

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0) return;

        var index = _grid.SelectedRows[0].Index;
        var rule = _rules[index];

        var confirm = MessageBox.Show(
            $"确定删除规则 \"{rule.Abbreviation}\"？",
            "确认删除",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            _rules.RemoveAt(index);
            SaveRules();
            RefreshGrid();
        }
    }

    // For testing: expose rule count
    public int RuleCount => _rules.Count;
    public bool IsEmptyLabelVisible => _lblEmpty.Visible;
}
