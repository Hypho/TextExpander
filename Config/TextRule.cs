namespace TextExpander.Config;

public class TextRule
{
    public string Abbr { get; set; } = "";
    public string Output { get; set; } = "";
    public bool UseRegex { get; set; } = false;
    public bool IMEBackspace { get; set; } = false;
    public string? ProcessFilter { get; set; }
    public bool Enabled { get; set; } = true;
}
