using System.Text.RegularExpressions;

namespace TextExpander.Config;

public class VariableResolver
{
    private readonly Func<string>? _clipboardProvider;

    public VariableResolver() : this(null) { }

    public VariableResolver(Func<string>? clipboardProvider)
    {
        _clipboardProvider = clipboardProvider;
    }

    public string Resolve(string text)
    {
        return ResolveInternal(text);
    }

    private string ResolveInternal(string text)
    {
        return Regex.Replace(text, @"\{(\w+)\}", match =>
        {
            var varName = match.Groups[1].Value.ToLowerInvariant();
            return varName switch
            {
                "date" => DateTime.Now.ToString("yyyy-MM-dd"),
                "time" => DateTime.Now.ToString("HH:mm:ss"),
                "clipboard" => _clipboardProvider?.Invoke() ?? GetClipboardText(),
                _ => match.Value
            };
        });
    }

    private static string GetClipboardText()
    {
        try
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
                return System.Windows.Forms.Clipboard.GetText();
        }
        catch
        {
            // Clipboard access can fail in some contexts
        }
        return string.Empty;
    }
}
