using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TextExpander.Hook;

public sealed class KeyboardHook : IDisposable
{
    private readonly Win32.LowLevelKeyboardProc _proc;
    private IntPtr _hookId = IntPtr.Zero;

    public event Action<uint>? KeyDown;

    public KeyboardHook()
    {
        _proc = HookCallback;
        using var module = Process.GetCurrentProcess().MainModule!;
        _hookId = Win32.SetWindowsHookEx(
            Win32.WH_KEYBOARD_LL, _proc,
            Win32.GetModuleHandle(module.ModuleName), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var info = Marshal.PtrToStructure<Win32.KBDLLHOOKSTRUCT>(lParam);
            bool isKeyDown = wParam == Win32.WM_KEYDOWN || wParam == Win32.WM_SYSKEYDOWN;
            bool isInjected = (info.flags & Win32.LLKHF_INJECTED) != 0;

            if (isKeyDown && !isInjected)
                KeyDown?.Invoke(info.vkCode);
        }
        return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        if (_hookId != IntPtr.Zero)
        {
            Win32.UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }
}
