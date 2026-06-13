using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Runtime.InteropServices;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class AlwaysOnTopViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isAlwaysOnTopEnabled;

    [ObservableProperty]
    private string _statusMessage = "A janela se comporta normalmente, ficando atrás de outras janelas.";

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;

    partial void OnIsAlwaysOnTopEnabledChanged(bool value)
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current.MainWindow);

        if (value)
        {
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            StatusMessage = "Always On Top está ATIVO. A janela ficará flutuando por cima das outras.";
        }
        else
        {
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            StatusMessage = "A janela se comporta normalmente, ficando atrás de outras janelas.";
        }
    }
}
