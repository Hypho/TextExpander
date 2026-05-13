namespace TextExpander.Config;

public class Rule
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Abbreviation { get; set; } = string.Empty;
    public string Replacement { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
    public string? UpdatedAt { get; set; }
}
