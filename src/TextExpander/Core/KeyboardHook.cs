using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TextExpander.Core;

public class KeyboardHook : IDisposable
{
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int VK_TAB = 0x09;
    private const int VK_SPACE = 0x20;
    private const int VK_RETURN = 0x0D;
    private const int VK_BACK = 0x08;

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private IntPtr _hookId = IntPtr.Zero;
    private readonly LowLevelKeyboardProc _proc;
    private readonly Action<char> _onCharTyped;
    private readonly Action<int> _onTerminatorKey;
    private readonly Action _onBackspace;
    private readonly Func<int, bool> _shouldSuppress;

    public bool IsInstalled => _hookId != IntPtr.Zero;

    public KeyboardHook(
        Action<char> onCharTyped,
        Action<int> onTerminatorKey,
        Action onBackspace,
        Func<int, bool> shouldSuppress)
    {
        _onCharTyped = onCharTyped;
        _onTerminatorKey = onTerminatorKey;
        _onBackspace = onBackspace;
        _shouldSuppress = shouldSuppress;
        _proc = HookCallback;
    }

    public bool Install()
    {
        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule;
        if (module == null) return false;

        var moduleHandle = GetModuleHandle(module.ModuleName);
        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, moduleHandle, 0);
        return _hookId != IntPtr.Zero;
    }

    public void Uninstall()
    {
        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            var hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            var vkCode = (int)hookStruct.vkCode;

            if (_shouldSuppress(vkCode))
                return (IntPtr)1;

            if (IsTerminatorKey(vkCode))
            {
                _onTerminatorKey(vkCode);
            }
            else if (vkCode == VK_BACK)
            {
                _onBackspace();
            }
            else if (vkCode >= 0x20 && vkCode <= 0x7E)
            {
                _onCharTyped((char)vkCode);
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static bool IsTerminatorKey(int vkCode) =>
        vkCode == VK_TAB || vkCode == VK_SPACE || vkCode == VK_RETURN;

    public void Dispose()
    {
        Uninstall();
    }
}
