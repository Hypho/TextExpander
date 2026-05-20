using TextExpander.UI;

namespace TextExpander.Tests.UI;

public class SettingsValidatorTests
{
    [Fact]
    [Trait("Category", "Smoke")]
    public void Validate_AllThreeKeys_ReturnsValid()
    {
        var result = SettingsValidator.ValidateTerminatorKeys(
            new List<string> { "Tab", "Space", "Enter" });
        Assert.Equal(SettingsValidationResult.Valid, result);
    }

    [Fact]
    public void Validate_SingleKey_ReturnsValid()
    {
        var result = SettingsValidator.ValidateTerminatorKeys(
            new List<string> { "Tab" });
        Assert.Equal(SettingsValidationResult.Valid, result);
    }

    [Fact]
    public void Validate_EmptyList_ReturnsNoTerminatorKeys()
    {
        var result = SettingsValidator.ValidateTerminatorKeys(
            new List<string>());
        Assert.Equal(SettingsValidationResult.NoTerminatorKeys, result);
    }
}
