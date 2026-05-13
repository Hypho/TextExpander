namespace TextExpander.Config;

public class AppConfig
{
    public List<string> TerminatorKeys { get; set; } = new() { "Tab", "Space", "Enter" };
    public bool StartOnBoot { get; set; }
    public bool Enabled { get; set; } = true;
}
