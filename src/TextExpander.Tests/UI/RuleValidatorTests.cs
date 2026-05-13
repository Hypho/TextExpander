using TextExpander.Config;
using TextExpander.UI;

namespace TextExpander.Tests.UI;

public class RuleValidatorTests
{
    private static List<Rule> EmptyRules() => new();

    private static List<Rule> RulesWith(string abbr)
    {
        return new List<Rule>
        {
            new() { Id = "existing-1", Abbreviation = abbr, Replacement = "text", Enabled = true }
        };
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void Validate_ValidInput_ReturnsValid()
    {
        var result = RuleValidator.Validate(";addr", "123 Main St", EmptyRules());
        Assert.Equal(ValidationResult.Valid, result);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void Validate_EmptyAbbreviation_ReturnsEmptyAbbreviation()
    {
        var result = RuleValidator.Validate("", "text", EmptyRules());
        Assert.Equal(ValidationResult.EmptyAbbreviation, result);
    }

    [Fact]
    public void Validate_WhitespaceAbbreviation_ReturnsEmptyAbbreviation()
    {
        var result = RuleValidator.Validate("   ", "text", EmptyRules());
        Assert.Equal(ValidationResult.EmptyAbbreviation, result);
    }

    [Fact]
    public void Validate_EmptyReplacement_ReturnsEmptyReplacement()
    {
        var result = RuleValidator.Validate(";addr", "", EmptyRules());
        Assert.Equal(ValidationResult.EmptyReplacement, result);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void Validate_DuplicateAbbreviation_ReturnsDuplicate()
    {
        var rules = RulesWith(";addr");
        var result = RuleValidator.Validate(";addr", "new text", rules);
        Assert.Equal(ValidationResult.DuplicateAbbreviation, result);
    }

    [Fact]
    public void Validate_DuplicateButEditingSameRule_ReturnsValid()
    {
        var rules = RulesWith(";addr");
        var result = RuleValidator.Validate(";addr", "updated text", rules, editingRuleId: "existing-1");
        Assert.Equal(ValidationResult.Valid, result);
    }

    [Fact]
    public void Validate_DifferentAbbreviation_ReturnsValid()
    {
        var rules = RulesWith(";addr");
        var result = RuleValidator.Validate(";email", "test@test.com", rules);
        Assert.Equal(ValidationResult.Valid, result);
    }
}
