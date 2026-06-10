using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DesktopCommandCenter.Infrastructure.Services;

public class GlobalHotkeyService : IHotkeyService, IDisposable
{
    // Win32 Imports
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateWindowEx(
        uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu,
        IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll")]
    private static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public WndProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private const uint WM_HOTKEY = 0x0312;
    private readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

    private readonly IntPtr _hwnd;
    private readonly WndProc _wndProcDelegate;
    private int _currentId = 1;
    private readonly Dictionary<int, Action> _callbacks = new();

    public GlobalHotkeyService()
    {
        _wndProcDelegate = WindowProc; // Garante que o GC não colete o delegate

        WNDCLASSEX wndClass = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
            lpfnWndProc = _wndProcDelegate,
            hInstance = GetModuleHandle(null!),
            lpszClassName = "GlobalHotkeyMessageWindow"
        };
        RegisterClassEx(ref wndClass);

        _hwnd = CreateWindowEx(0, "GlobalHotkeyMessageWindow", "GlobalHotkey", 0, 0, 0, 0, 0, HWND_MESSAGE, IntPtr.Zero, wndClass.hInstance, IntPtr.Zero);
    }

    public bool RegisterHotkey(int modifier, int key, Action callback)
    {
        int id = _currentId++;
        if (RegisterHotKey(_hwnd, id, (uint)modifier, (uint)key))
        {
            _callbacks[id] = callback;
            return true;
        }
        return false;
    }

    public void UnregisterAll()
    {
        foreach (var id in _callbacks.Keys)
        {
            UnregisterHotKey(_hwnd, id);
        }
        _callbacks.Clear();
    }

    private IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_HOTKEY)
        {
            int id = wParam.ToInt32();
            if (_callbacks.TryGetValue(id, out var action))
            {
                // Trigger the action on the current thread
                action();
            }
        }
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    public static string GetHotkeyString(uint modifiers, uint virtualKey)
    {
        if (virtualKey == 0) return "None";

        var parts = new List<string>();
        if ((modifiers & 2) != 0) parts.Add("Ctrl");
        if ((modifiers & 1) != 0) parts.Add("Alt");
        if ((modifiers & 4) != 0) parts.Add("Shift");
        if ((modifiers & 8) != 0) parts.Add("Win");

        // Convert virtual key to char if possible, or string
        parts.Add(((ConsoleKey)virtualKey).ToString());

        return string.Join(" + ", parts);
    }

    public void Dispose()
    {
        UnregisterAll();
    }
}
