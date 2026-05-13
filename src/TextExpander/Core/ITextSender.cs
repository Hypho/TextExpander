namespace TextExpander.Core;

public interface ITextSender
{
    void SendBackspaces(int count);
    void SendText(string text);
}
