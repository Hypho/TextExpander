using TextExpander.Config;
using TextExpander.Core;

namespace TextExpander.Tests.Core;

public class TextReplacerTests
{
    private class FakeTextSender : ITextSender
    {
        public int BackspaceCount { get; private set; }
        public string? SentText { get; private set; }

        public void SendBackspaces(int count) => BackspaceCount = count;
        public void SendText(string text) => SentText = text;
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void Replace_DeletesAbbreviation_SendsReplacement()
    {
        var sender = new FakeTextSender();
        var resolver = new VariableResolver();
        var replacer = new TextReplacer(sender, resolver);

        replacer.Replace(";addr", "123 Main St");

        Assert.Equal(5, sender.BackspaceCount); // ";addr".Length
        Assert.Equal("123 Main St", sender.SentText);
    }

    [Fact]
    public void Replace_WithVariable_ResolvesBeforeSending()
    {
        var sender = new FakeTextSender();
        var resolver = new VariableResolver(() => "clip-text");
        var replacer = new TextReplacer(sender, resolver);

        replacer.Replace(";c", "{clipboard}");

        Assert.Equal(2, sender.BackspaceCount);
        Assert.Equal("clip-text", sender.SentText);
    }

    [Fact]
    public void Replace_EmptyReplacement_SendsBackspacesOnly()
    {
        var sender = new FakeTextSender();
        var resolver = new VariableResolver();
        var replacer = new TextReplacer(sender, resolver);

        replacer.Replace(";x", "");

        Assert.Equal(2, sender.BackspaceCount);
        Assert.Equal("", sender.SentText);
    }
}
