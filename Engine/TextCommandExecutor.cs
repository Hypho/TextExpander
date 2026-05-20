using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TextExpander.Config;
using TextExpander.Hook;

namespace TextExpander.Engine;

public class TextCommandExecutor
{
    private const uint VK_BACK = 0x08;
    private const uint VK_ESCAPE = 0x1B;

    public void Erase(TextRule rule, string captured)
    {
        if (rule.IMEBackspace)
            SendBackspace(captured.Length);
        else
            SendKey(VK_ESCAPE);
    }

    public void Output(string text)
    {
        text = ProcessTemplates(text);

        var inputs = new List<Win32.INPUT>();
        foreach (char c in text)
        {
            inputs.Add(MakeUnicodeKey(c, keyUp: false));
            inputs.Add(MakeUnicodeKey(c, keyUp: true));
        }
        Win32.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf<Win32.INPUT>());
    }

    private void SendBackspace(int count)
    {
        var inputs = new Win32.INPUT[count * 2];
        for (int i = 0; i < count; i++)
        {
            inputs[i * 2] = MakeVkKey(VK_BACK, keyUp: false);
            inputs[i * 2 + 1] = MakeVkKey(VK_BACK, keyUp: true);
        }
        Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Win32.INPUT>());
    }

    private void SendKey(uint vk)
    {
        var inputs = new[] { MakeVkKey(vk, false), MakeVkKey(vk, true) };
        Win32.SendInput(2, inputs, Marshal.SizeOf<Win32.INPUT>());
    }

    public void SendBackspaceForTriggerKey()
    {
        SendBackspace(1);
    }

    private static Win32.INPUT MakeUnicodeKey(char c, bool keyUp) => new()
    {
        type = 1,
        ki = new Win32.KEYBDINPUT
        {
            wVk = 0,
            wScan = c,
            dwFlags = Win32.KEYEVENTF_UNICODE | (keyUp ? (uint)Win32.KEYEVENTF_KEYUP : 0u)
        }
    };

    private static Win32.INPUT MakeVkKey(uint vk, bool keyUp) => new()
    {
        type = 1,
        ki = new Win32.KEYBDINPUT
        {
            wVk = (ushort)vk,
            dwFlags = keyUp ? Win32.KEYEVENTF_KEYUP : 0u
        }
    };

    private static string ProcessTemplates(string text)
    {
        return Regex.Replace(text, @"\{DATE:([^}]+)\}",
            m => DateTime.Now.ToString(m.Groups[1].Value));
    }
}
