using System.Text.RegularExpressions;
using TextExpander.Config;

namespace TextExpander.Engine;

public class RuleMatcher
{
    private readonly RuleStore _store;

    public RuleMatcher(RuleStore store) => _store = store;

    public (TextRule rule, string captured)? TryMatch(string bufferTail, string? activeProcess)
    {
        foreach (var rule in _store.Rules)
        {
            if (!rule.Enabled) continue;
            if (rule.ProcessFilter != null &&
                !string.Equals(activeProcess, rule.ProcessFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            if (rule.UseRegex)
            {
                var m = Regex.Match(bufferTail, rule.Abbr + "$");
                if (m.Success)
                    return (rule, m.Value);
            }
            else
            {
                if (bufferTail.EndsWith(rule.Abbr, StringComparison.Ordinal))
                    return (rule, rule.Abbr);
            }
        }
        return null;
    }
}
