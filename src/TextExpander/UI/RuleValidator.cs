using TextExpander.Config;

namespace TextExpander.UI;

public enum ValidationResult { Valid, EmptyAbbreviation, EmptyReplacement, DuplicateAbbreviation }

public static class RuleValidator
{
    public static ValidationResult Validate(string abbreviation, string replacement, List<Rule> existingRules, string? editingRuleId = null)
    {
        if (string.IsNullOrWhiteSpace(abbreviation))
            return ValidationResult.EmptyAbbreviation;

        if (string.IsNullOrWhiteSpace(replacement))
            return ValidationResult.EmptyReplacement;

        var duplicate = existingRules.FirstOrDefault(r =>
            r.Abbreviation == abbreviation && r.Id != editingRuleId);
        if (duplicate != null)
            return ValidationResult.DuplicateAbbreviation;

        return ValidationResult.Valid;
    }
}
