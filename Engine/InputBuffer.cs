using System.Text;

namespace TextExpander.Engine;

public class InputBuffer
{
    private readonly int _maxLength;
    private readonly StringBuilder _buf = new();

    public InputBuffer(int maxLength = 64) => _maxLength = maxLength;

    public string Current => _buf.ToString();

    public void Push(char c)
    {
        _buf.Append(c);
        if (_buf.Length > _maxLength)
            _buf.Remove(0, 1);
    }

    public void PopBack()
    {
        if (_buf.Length > 0)
            _buf.Remove(_buf.Length - 1, 1);
    }

    public void Reset() => _buf.Clear();
}
