using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System;
using System.IO;
using DesktopCommandCenter.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Controls;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class FutureShellWindow : Window
{
    private DispatcherTimer _hudTimer;
    private int _tabCount = 0;

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

    private class TerminalTabContext
    {
        public Microsoft.UI.Xaml.Controls.WebView2 WebView { get; set; } = null!;
        public ITerminalService TerminalService { get; set; } = null!;
        public bool IsInitialized { get; set; }
    }

    public FutureShellWindow()
    {
        InitializeComponent();
        
        // MUST BE SET BEFORE AppWindow properties to avoid FailFast crash on DPI changes
        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(RightDragRegion);
        
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        try { SetCurrentProcessExplicitAppUserModelID("DCC.FutureShell"); } catch { }

        appWindow.Title = "FutureShell";
        var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "FutureShell-IconSemTexto.ico");
        if (!File.Exists(iconPath))
        {
            iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "AppIcon.ico");
        }
        
        if (File.Exists(iconPath))
        {
            appWindow.SetIcon(iconPath);
        }

        ApplyTitleBarColors();

        _hudTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _hudTimer.Tick += HudTimer_Tick;
        _hudTimer.Start();

        // Create the first tab
        CreateNewTab();
    }

    [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    private void ApplyTitleBarColors()
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        
        bool isDark = false;
        if (Content is FrameworkElement root)
        {
            var themeStr = App.GetTheme();
            isDark = themeStr == "Dark" || (themeStr != "Light" && App.Current.RequestedTheme == ApplicationTheme.Dark);
        }
        
        int isDarkValue = isDark ? 1 : 0;
        DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref isDarkValue, sizeof(int));

        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        var titleBar = appWindow?.TitleBar;
        if (titleBar == null) return;

        if (isDark)
        {
            titleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
            titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
            titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
            titleBar.ButtonInactiveForegroundColor = Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x80, 0x80, 0x80);
        }
        else
        {
            titleBar.ButtonForegroundColor = Microsoft.UI.Colors.Black;
            titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Black;
            titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.Black;
            titleBar.ButtonInactiveForegroundColor = Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x80, 0x80, 0x80);
        }

        titleBar.ButtonBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(1, 0, 0, 0);
        titleBar.ButtonHoverBackgroundColor = isDark
            ? Microsoft.UI.ColorHelper.FromArgb(0x20, 0xFF, 0xFF, 0xFF)
            : Microsoft.UI.ColorHelper.FromArgb(0x20, 0x00, 0x00, 0x00);
        titleBar.ButtonPressedBackgroundColor = isDark
            ? Microsoft.UI.ColorHelper.FromArgb(0x40, 0xFF, 0xFF, 0xFF)
            : Microsoft.UI.ColorHelper.FromArgb(0x40, 0x00, 0x00, 0x00);
        titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(1, 0, 0, 0);
    }

    private void HudTimer_Tick(object? sender, object e)
    {
        if (TerminalTabView.SelectedItem is TabViewItem selectedTab && selectedTab.Tag is TerminalTabContext context && context.WebView?.CoreWebView2 != null)
        {
            try 
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();
                var ramMB = (process.WorkingSet64 / 1024 / 1024).ToString();
                
                var msg = new { type = "hud", cpu = "~1%", ram = ramMB, ai = "Ready" };
                var json = JsonSerializer.Serialize(msg);
                context.WebView.CoreWebView2.PostWebMessageAsJson(json);
            }
            catch { }
        }
    }

    private void TabView_AddTabButtonClick(TabView sender, object args)
    {
        CreateNewTab();
    }

    private async void CreateNewTab()
    {
        _tabCount++;
        
        var webView = new Microsoft.UI.Xaml.Controls.WebView2 
        { 
            DefaultBackgroundColor = Microsoft.UI.Colors.Black,
            HorizontalAlignment = HorizontalAlignment.Stretch, 
            VerticalAlignment = VerticalAlignment.Stretch 
        };

        var terminalService = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<ITerminalService>();

        var context = new TerminalTabContext 
        { 
            WebView = webView,
            TerminalService = terminalService
        };

        var newTab = new TabViewItem
        {
            Header = $"Terminal {_tabCount}",
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource { Symbol = Symbol.Document },
            Content = webView,
            Tag = context
        };

        TerminalTabView.TabItems.Add(newTab);
        TerminalTabView.SelectedItem = newTab;

        terminalService.OutputDataReceived += (s, e) => 
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (webView.CoreWebView2 != null)
                {
                    try 
                    {
                        var msg = new { type = "output", data = e };
                        var json = JsonSerializer.Serialize(msg);
                        webView.CoreWebView2.PostWebMessageAsJson(json);
                    } 
                    catch { }
                }
            });
        };

        terminalService.ProcessExited += (s, e) => 
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                CloseTab(newTab);
            });
        };

        await webView.EnsureCoreWebView2Async();
        
        var terminalHtmlPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Terminal", "terminal.html");
        webView.CoreWebView2.Navigate($"file:///{terminalHtmlPath.Replace('\\', '/')}");
        
        webView.CoreWebView2.WebMessageReceived += async (s, args) => 
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
                        await terminalService.WriteInputAsync(dataProp.GetString() ?? "");
                    }
                    else if (type == "resize")
                    {
                        if (root.TryGetProperty("cols", out var colsProp) && root.TryGetProperty("rows", out var rowsProp))
                        {
                            terminalService.Resize(colsProp.GetInt32(), rowsProp.GetInt32());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing input: {ex.Message}");
            }
        };

        webView.NavigationCompleted += async (s, args) => 
        {
            if (context.IsInitialized) return;
            context.IsInitialized = true;

            var isPt = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("pt", StringComparison.OrdinalIgnoreCase);
            string banner = isPt 
                ? "O Future Shell foi desenvolvido por Foreign Technologies..." 
                : "The FutureShell was developed by Foreign Technologies...";
                
            try 
            {
                var initScriptPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Terminal", "FutureShellInit.ps1");
                var pshCommand = $"powershell.exe -NoLogo -NoExit -ExecutionPolicy Bypass -File \"{initScriptPath}\"";
                await terminalService.StartAsync(pshCommand, 100, 30);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to start terminal: {ex.Message}");
            }
        };
    }

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        CloseTab(args.Tab);
    }

    private void CloseTab(TabViewItem tab)
    {
        if (tab.Tag is TerminalTabContext context)
        {
            try
            {
                context.TerminalService.Stop();
                context.TerminalService.Dispose();
            }
            catch { }
            
            try
            {
                context.WebView.Close();
            }
            catch { }
        }

        TerminalTabView.TabItems.Remove(tab);

        // If no tabs left, close window
        if (TerminalTabView.TabItems.Count == 0)
        {
            this.Close();
        }
    }

    private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Focus the webview of the selected tab when switched
        if (TerminalTabView.SelectedItem is TabViewItem selectedTab && selectedTab.Content is Microsoft.UI.Xaml.Controls.WebView2 webView)
        {
            webView.Focus(FocusState.Programmatic);
        }
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        if (_hudTimer != null)
        {
            _hudTimer.Stop();
            _hudTimer.Tick -= HudTimer_Tick;
        }

        // Close all tabs to clean up processes
        var tabs = new System.Collections.Generic.List<TabViewItem>();
        foreach (TabViewItem tab in TerminalTabView.TabItems)
        {
            tabs.Add(tab);
        }

        foreach (var tab in tabs)
        {
            CloseTab(tab);
        }
    }
}
