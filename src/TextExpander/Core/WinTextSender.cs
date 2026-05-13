using System.Runtime.InteropServices;

namespace TextExpander.Core;

public class WinTextSender : ITextSender
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern short VkKeyScan(char ch);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public INPUTUNION union;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct INPUTUNION
    {
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private const uint INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_UNICODE = 0x0004;

    public void SendBackspaces(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SendKey(0x08); // VK_BACK
        }
    }

    public void SendText(string text)
    {
        foreach (var c in text)
        {
            if (c == '\n')
            {
                SendKey(0x0D); // VK_RETURN
            }
            else if (c == '\r')
            {
                // skip \r in \r\n
            }
            else
            {
                SendUnicodeChar(c);
            }
        }
    }

    private static void SendKey(ushort vk)
    {
        var down = new INPUT { type = INPUT_KEYBOARD, union = new INPUTUNION { ki = new KEYBDINPUT { wVk = vk } } };
        var up = new INPUT { type = INPUT_KEYBOARD, union = new INPUTUNION { ki = new KEYBDINPUT { wVk = vk, dwFlags = KEYEVENTF_KEYUP } } };
        var inputs = new[] { down, up };
        SendInput(2, inputs, Marshal.SizeOf<INPUT>());
    }

    private static void SendUnicodeChar(char c)
    {
        var down = new INPUT
        {
            type = INPUT_KEYBOARD,
            union = new INPUTUNION { ki = new KEYBDINPUT { wScan = c, dwFlags = KEYEVENTF_UNICODE } }
        };
        var up = new INPUT
        {
            type = INPUT_KEYBOARD,
            union = new INPUTUNION { ki = new KEYBDINPUT { wScan = c, dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP } }
        };
        var inputs = new[] { down, up };
        SendInput(2, inputs, Marshal.SizeOf<INPUT>());
    }
}
