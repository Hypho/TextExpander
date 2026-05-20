namespace TextExpander.UI;

public enum SettingsValidationResult { Valid, NoTerminatorKeys }

public static class SettingsValidator
{
    public static SettingsValidationResult ValidateTerminatorKeys(List<string> keys)
    {
        return keys.Count > 0
            ? SettingsValidationResult.Valid
            : SettingsValidationResult.NoTerminatorKeys;
    }
}
