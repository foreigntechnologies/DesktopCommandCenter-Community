using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using Windows.UI;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ColorLoupeWindow : Window
{
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;
    private const uint WS_EX_TOPMOST = 0x00000008;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;

    public ColorLoupeWindow()
    {
        this.InitializeComponent();

        this.ExtendsContentIntoTitleBar = true; // Remove standard title bar
        
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        
        // Make window topmost, click-through, and not appear in taskbar
        uint exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST | WS_EX_TOOLWINDOW);

        // AppWindow configuration
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        
        var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
        if (presenter != null)
        {
            presenter.IsAlwaysOnTop = true;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);
        }

        appWindow.Resize(new Windows.Graphics.SizeInt32(100, 100));
    }

    public void MoveTo(int x, int y)
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        // Position slightly offset from cursor so it doesn't block exactly what we are picking
        appWindow.Move(new Windows.Graphics.PointInt32(x + 15, y + 15));
    }

    public void UpdateColor(string hex, byte r, byte g, byte b)
    {
        ColorBrush.Color = Color.FromArgb(255, r, g, b);
        HexText.Text = hex;
    }
}
