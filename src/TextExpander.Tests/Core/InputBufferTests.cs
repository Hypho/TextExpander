using TextExpander.Core;

namespace TextExpander.Tests.Core;

public class InputBufferTests
{
    [Fact]
    [Trait("Category", "Smoke")]
    public void Append_SingleChar_AddsToBuffer()
    {
        var buffer = new InputBuffer(maxLength: 50);
        buffer.Append('a');
        Assert.Equal("a", buffer.Content);
    }

    [Fact]
    public void Append_MultipleChars_Accumulates()
    {
        var buffer = new InputBuffer(maxLength: 50);
        buffer.Append('h');
        buffer.Append('e');
        buffer.Append('l');
        buffer.Append('l');
        buffer.Append('o');
        Assert.Equal("hello", buffer.Content);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void Append_ExceedsMaxLength_TruncatesOldest()
    {
        var buffer = new InputBuffer(maxLength: 5);
        foreach (var c in "abcdefgh") buffer.Append(c);
        Assert.Equal("defgh", buffer.Content);
    }

    [Fact]
    public void Clear_ResetsBuffer()
    {
        var buffer = new InputBuffer(maxLength: 50);
        buffer.Append('a');
        buffer.Append('b');
        buffer.Clear();
        Assert.Equal("", buffer.Content);
    }

    [Fact]
    public void Backspace_RemovesLastChar()
    {
        var buffer = new InputBuffer(maxLength: 50);
        buffer.Append('a');
        buffer.Append('b');
        buffer.Append('c');
        buffer.Backspace();
        Assert.Equal("ab", buffer.Content);
    }

    [Fact]
    public void Backspace_EmptyBuffer_DoesNothing()
    {
        var buffer = new InputBuffer(maxLength: 50);
        buffer.Backspace();
        Assert.Equal("", buffer.Content);
    }

    [Fact]
    public void Content_NewBuffer_ReturnsEmptyString()
    {
        var buffer = new InputBuffer(maxLength: 50);
        Assert.Equal("", buffer.Content);
    }

    [Fact]
    public void RemoveLastN_RemovesSpecifiedChars()
    {
        var buffer = new InputBuffer(maxLength: 50);
        foreach (var c in "hello") buffer.Append(c);
        buffer.RemoveLastN(3);
        Assert.Equal("he", buffer.Content);
    }

    [Fact]
    public void RemoveLastN_MoreThanBuffer_ClearsBuffer()
    {
        var buffer = new InputBuffer(maxLength: 50);
        buffer.Append('a');
        buffer.RemoveLastN(10);
        Assert.Equal("", buffer.Content);
    }
}
