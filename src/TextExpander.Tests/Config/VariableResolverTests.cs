using TextExpander.Config;

namespace TextExpander.Tests.Config;

public class VariableResolverTests
{
    private readonly VariableResolver _resolver = new();

    [Fact]
    [Trait("Category", "Smoke")]
    public void Resolve_DateVariable_ReplacesWithCurrentDate()
    {
        var result = _resolver.Resolve("Today is {date}");
        var expected = $"Today is {DateTime.Now:yyyy-MM-dd}";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Resolve_TimeVariable_ReplacesWithCurrentTime()
    {
        var result = _resolver.Resolve("Time: {time}");
        var parts = result.Split("Time: ");
        Assert.Equal(2, parts.Length);
        Assert.Contains(":", parts[1]);
    }

    [Fact]
    public void Resolve_ClipboardVariable_ReplacesWithClipboardText()
    {
        var resolver = new VariableResolver(() => "clipboard-content");
        var result = resolver.Resolve("Copy: {clipboard}");
        Assert.Equal("Copy: clipboard-content", result);
    }

    [Fact]
    public void Resolve_ClipboardEmpty_ReplacesWithEmptyString()
    {
        var resolver = new VariableResolver(() => "");
        var result = resolver.Resolve("Copy: {clipboard}");
        Assert.Equal("Copy: ", result);
    }

    [Fact]
    public void Resolve_UnknownVariable_KeepsAsIs()
    {
        var result = _resolver.Resolve("Value: {unknown}");
        Assert.Equal("Value: {unknown}", result);
    }

    [Fact]
    public void Resolve_NoVariables_ReturnsOriginal()
    {
        var result = _resolver.Resolve("No variables here");
        Assert.Equal("No variables here", result);
    }

    [Fact]
    public void Resolve_MultipleVariables_ResolvesAll()
    {
        var resolver = new VariableResolver(() => "clip");
        var result = resolver.Resolve("{date} {time} {clipboard}");
        var parts = result.Split(' ');
        Assert.Equal(3, parts.Length);
        Assert.Contains("-", parts[0]); // date has dashes
        Assert.Contains(":", parts[1]); // time has colons
        Assert.Equal("clip", parts[2]);
    }
}
