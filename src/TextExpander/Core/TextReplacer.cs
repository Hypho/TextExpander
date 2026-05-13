using TextExpander.Config;

namespace TextExpander.Core;

public class TextReplacer
{
    private readonly ITextSender _sender;
    private readonly VariableResolver _resolver;

    public TextReplacer(ITextSender sender, VariableResolver resolver)
    {
        _sender = sender;
        _resolver = resolver;
    }

    public void Replace(string abbreviation, string replacement)
    {
        var resolved = _resolver.Resolve(replacement);
        _sender.SendBackspaces(abbreviation.Length);
        _sender.SendText(resolved);
    }
}
