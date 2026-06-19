using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System;
using System.IO;
using DesktopCommandCenter.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class FutureShellWindow : Window
{
    private ITerminalService _terminalService;
    private DispatcherTimer _hudTimer;

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

    public FutureShellWindow()
    {
        InitializeComponent();
        
        // Setup Window Icon and TitleBar
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        try { SetCurrentProcessExplicitAppUserModelID("DCC.FutureShell"); } catch { }

        appWindow.Title = "FutureShell";
        var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "FutureShell.ico");
        if (File.Exists(iconPath))
        {
            appWindow.SetIcon(iconPath);
        }

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppTitleBar.Loaded += AppTitleBar_Loaded;
        }
        else
        {
            AppTitleBar.Visibility = Visibility.Collapsed;
        }

        // Initialize Terminal Service
        _terminalService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<ITerminalService>();
        _terminalService.OutputDataReceived += TerminalService_OutputDataReceived;

        _hudTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _hudTimer.Tick += HudTimer_Tick;
        _hudTimer.Start();

        InitializeTerminalAsync();
    }

    private void HudTimer_Tick(object? sender, object e)
    {
        if (TerminalWebView.CoreWebView2 != null)
        {
            try 
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();
                var ramMB = (process.WorkingSet64 / 1024 / 1024).ToString();
                
                var msg = new { type = "hud", cpu = "~1%", ram = ramMB, ai = "Ready" };
                var json = JsonSerializer.Serialize(msg);
                TerminalWebView.CoreWebView2.PostWebMessageAsJson(json);
            }
            catch { }
        }
    }

    private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            appWindow.TitleBar.SetDragRectangles(new[] {
                new Windows.Graphics.RectInt32(0, 0, (int)AppTitleBar.ActualWidth, (int)AppTitleBar.ActualHeight)
            });
        }
    }

    private async void InitializeTerminalAsync()
    {
        await TerminalWebView.EnsureCoreWebView2Async();
        
        var terminalHtmlPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Terminal", "terminal.html");
        TerminalWebView.CoreWebView2.Navigate($"file:///{terminalHtmlPath.Replace('\\', '/')}");
        
        TerminalWebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        TerminalWebView.NavigationCompleted += TerminalWebView_NavigationCompleted;
    }

    private async void TerminalWebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
    {
        // Start PowerShell backend once UI is loaded
        try 
        {
            await _terminalService.StartAsync("powershell.exe", 100, 30);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to start terminal: {ex.Message}");
        }
    }

    private async void CoreWebView2_WebMessageReceived(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
    {
        try
        {
            string json = args.WebMessageAsJson;
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.TryGetProperty("type", out var typeProp))
            {
                var type = typeProp.GetString();
                if (type == "input" && root.TryGetProperty("data", out var dataProp))
                {
                    await _terminalService.WriteInputAsync(dataProp.GetString() ?? "");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error processing input: {ex.Message}");
        }
    }

    private void TerminalService_OutputDataReceived(object? sender, string e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (TerminalWebView.CoreWebView2 != null)
            {
                try 
                {
                    var msg = new { type = "output", data = e };
                    var json = JsonSerializer.Serialize(msg);
                    TerminalWebView.CoreWebView2.PostWebMessageAsJson(json);
                } 
                catch { }
            }
        });
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        try
        {
            _terminalService.OutputDataReceived -= TerminalService_OutputDataReceived;
            _terminalService.Stop();
        }
        catch { }
    }
}
