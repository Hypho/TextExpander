using TextExpander.Config;
using TextExpander.Core;

namespace TextExpander.Tests.Core;

public class AbbreviationMatcherTests
{
    private static List<Rule> CreateRules(params (string abbr, string replacement)[] entries)
    {
        return entries.Select(e => new Rule
        {
            Id = Guid.NewGuid().ToString(),
            Abbreviation = e.abbr,
            Replacement = e.replacement,
            Enabled = true
        }).ToList();
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void TryMatch_ExactMatch_ReturnsRule()
    {
        var rules = CreateRules((";addr", "123 Main St"));
        var matcher = new AbbreviationMatcher(rules);

        var result = matcher.TryMatch(";addr");

        Assert.NotNull(result);
        Assert.Equal("123 Main St", result!.Replacement);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void TryMatch_NoMatch_ReturnsNull()
    {
        var rules = CreateRules((";addr", "123 Main St"));
        var matcher = new AbbreviationMatcher(rules);

        var result = matcher.TryMatch(";unknown");

        Assert.Null(result);
    }

    [Fact]
    public void TryMatch_PartialMatch_ReturnsNull()
    {
        var rules = CreateRules((";addr", "123 Main St"));
        var matcher = new AbbreviationMatcher(rules);

        var result = matcher.TryMatch(";add");

        Assert.Null(result);
    }

    [Fact]
    public void TryMatch_DisabledRule_ReturnsNull()
    {
        var rule = new Rule { Abbreviation = ";addr", Replacement = "123", Enabled = false };
        var matcher = new AbbreviationMatcher(new List<Rule> { rule });

        var result = matcher.TryMatch(";addr");

        Assert.Null(result);
    }

    [Fact]
    public void TryMatch_MultipleRulesSameAbbr_ReturnsFirst()
    {
        var rules = CreateRules((";x", "first"), (";x", "second"));
        var matcher = new AbbreviationMatcher(rules);

        var result = matcher.TryMatch(";x");

        Assert.NotNull(result);
        Assert.Equal("first", result!.Replacement);
    }

    [Fact]
    public void TryMatch_EmptyRules_ReturnsNull()
    {
        var matcher = new AbbreviationMatcher(new List<Rule>());

        var result = matcher.TryMatch(";anything");

        Assert.Null(result);
    }

    [Fact]
    public void ReloadRules_UpdatesMatchers()
    {
        var rules1 = CreateRules((";a", "alpha"));
        var matcher = new AbbreviationMatcher(rules1);
        Assert.NotNull(matcher.TryMatch(";a"));

        var rules2 = CreateRules((";b", "beta"));
        matcher.ReloadRules(rules2);
        Assert.Null(matcher.TryMatch(";a"));
        Assert.NotNull(matcher.TryMatch(";b"));
    }

    [Fact]
    public void MaxAbbreviationLength_ReturnsLongestAbbr()
    {
        var rules = CreateRules((";a", "1"), (";longabbr", "2"), (";mid", "3"));
        var matcher = new AbbreviationMatcher(rules);

        Assert.Equal(9, matcher.MaxAbbreviationLength); // ";longabbr" = 9 chars
    }
}
