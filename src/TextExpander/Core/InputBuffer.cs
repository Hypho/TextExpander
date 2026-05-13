using System.Text;

namespace TextExpander.Core;

public class InputBuffer
{
    private readonly int _maxLength;
    private readonly StringBuilder _buffer = new();

    public InputBuffer(int maxLength)
    {
        _maxLength = maxLength;
    }

    public string Content => _buffer.ToString();

    public void Append(char c)
    {
        _buffer.Append(c);
        if (_buffer.Length > _maxLength)
            _buffer.Remove(0, _buffer.Length - _maxLength);
    }

    public void Backspace()
    {
        if (_buffer.Length > 0)
            _buffer.Remove(_buffer.Length - 1, 1);
    }

    public void Clear()
    {
        _buffer.Clear();
    }

    public void RemoveLastN(int n)
    {
        var count = Math.Min(n, _buffer.Length);
        if (count > 0)
            _buffer.Remove(_buffer.Length - count, count);
    }
}
