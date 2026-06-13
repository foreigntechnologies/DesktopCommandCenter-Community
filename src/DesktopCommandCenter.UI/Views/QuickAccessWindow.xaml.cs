using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using CommunityToolkit.Mvvm.Messaging;
using WinRT.Interop;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class QuickAccessWindow : Window
{
    // ── Win32 P/Invoke ────────────────────────────────────────────────────────
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out POINT lpPoint);
    [DllImport("user32.dll")] private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
    [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
    [DllImport("user32.dll")] private static extern int GetDpiForWindow(IntPtr hwnd);

    [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x, y; }
    [StructLayout(LayoutKind.Sequential)] private struct RECT  { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)] private struct MONITORINFO
    {
        public uint cbSize; public RECT rcMonitor; public RECT rcWork; public uint dwFlags;
    }
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    // ─────────────────────────────────────────────────────────────────────────

    private const int PanelWidth  = 380;
    private const int PanelHeight = 480;

    public QuickAccessWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon("Assets/AppIcon.ico");

        // Remove a barra de título nativa
        ExtendsContentIntoTitleBar = true;

        // Acrylic backdrop
        SystemBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();

        // Sem botões de título, sem redimensionamento, sem maximizar
        if (AppWindow.Presenter is OverlappedPresenter p)
        {
            p.IsResizable      = false;
            p.IsMaximizable    = false;
            p.IsMinimizable    = false;
            p.IsAlwaysOnTop    = true;
            p.SetBorderAndTitleBar(false, false); // Sem borda e sem titlebar nativos
        }

        // Fecha ao perder o foco
        this.Activated += (s, e) =>
        {
            if (e.WindowActivationState == WindowActivationState.Deactivated)
            {
                this.AppWindow.Hide();
            }
        };

        // Versão no badge inferior
        TxtVersion.Text = $"v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.1"}";
    }

    /// <summary>
    /// Posiciona e exibe o painel no canto inferior direito da tela onde está o cursor/bandeja.
    /// </summary>
    public void ShowAtTray()
    {
        PositionNearTray();
        AppWindow.Show();
        this.Activate();
    }

    private void PositionNearTray()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        double dpiScale = GetDpiForWindow(hwnd) / 96.0;

        // Descobre o monitor onde fica a bandeja (usa o cursor como referência)
        GetCursorPos(out var pt);
        var hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
        var mi = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
        GetMonitorInfo(hMonitor, ref mi);

        // Área de trabalho do monitor (descontando a barra de tarefas)
        int workRight  = mi.rcWork.Right;
        int workBottom = mi.rcWork.Bottom;

        // Converte tamanho lógico do painel para físico (pixels)
        int physW = (int)(PanelWidth  * dpiScale);
        int physH = (int)(PanelHeight * dpiScale);

        int x = workRight  - physW - 12;
        int y = workBottom - physH - 12;

        AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, physW, physH));
    }

    // ── Handlers de navegação ─────────────────────────────────────────────────

    private void Tool_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string actionId)
        {
            AppWindow.Hide();
            App.Current.MainWindow?.AppWindow.Show();
            WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage(actionId));
        }
    }

    private void BtnMais_Click(object sender, RoutedEventArgs e)
    {
        AppWindow.Hide();
        var mainWin = App.Current.MainWindow;
        mainWin?.AppWindow.Show();
        if (mainWin?.AppWindow.Presenter is OverlappedPresenter p)
            p.Maximize();
    }

    private async void BtnDocs_Click(object sender, RoutedEventArgs e)
    {
        AppWindow.Hide();
        await Windows.System.Launcher.LaunchUriAsync(
            new Uri("https://github.com/foreigntechnologies/DesktopCommandCenter-Community"));
    }

    private async void BtnBug_Click(object sender, RoutedEventArgs e)
    {
        AppWindow.Hide();
        App.Current.MainWindow?.AppWindow.Show();

        // Usa o WeakReferenceMessenger para pedir ao MainWindow que abra o BugDialog
        WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("BugReport"));
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        AppWindow.Hide();
        App.Current.MainWindow?.AppWindow.Show();
        WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("Settings"));
    }
}
