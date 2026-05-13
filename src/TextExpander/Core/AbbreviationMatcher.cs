using TextExpander.Config;

namespace TextExpander.Core;

public class AbbreviationMatcher
{
    private List<Rule> _rules;
    private readonly Dictionary<string, Rule> _enabledRules = new();

    public AbbreviationMatcher(List<Rule> rules)
    {
        _rules = rules;
        RebuildIndex();
    }

    public int MaxAbbreviationLength { get; private set; }

    public Rule? TryMatch(string input)
    {
        return _enabledRules.TryGetValue(input, out var rule) ? rule : null;
    }

    public void ReloadRules(List<Rule> rules)
    {
        _rules = rules;
        RebuildIndex();
    }

    private void RebuildIndex()
    {
        _enabledRules.Clear();
        MaxAbbreviationLength = 0;

        foreach (var rule in _rules)
        {
            if (!rule.Enabled) continue;

            if (!_enabledRules.ContainsKey(rule.Abbreviation))
                _enabledRules[rule.Abbreviation] = rule;

            if (rule.Abbreviation.Length > MaxAbbreviationLength)
                MaxAbbreviationLength = rule.Abbreviation.Length;
        }
    }
}
