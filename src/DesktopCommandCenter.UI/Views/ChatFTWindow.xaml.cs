using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ChatFTWindow : Window
{
    [DllImport("shell32.dll", SetLastError = true)]
    public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

    public ChatFTWindow()
    {
        this.InitializeComponent();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        try { SetCurrentProcessExplicitAppUserModelID("DCC.ChatFT"); } catch { }

        appWindow.Title = "ChatFT";
        var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "ChatFT-LogoSemTexto.ico");
        if (!File.Exists(iconPath))
        {
            iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "DCCAppIcon.ico");
        }

        if (File.Exists(iconPath))
        {
            appWindow.SetIcon(iconPath);
        }

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        }
        else
        {
            AppTitleBar.Visibility = Visibility.Collapsed;
        }

        // Apply theme
        var theme = App.GetTheme();
        if (this.Content is FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = theme switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };

            var isDark = frameworkElement.RequestedTheme == ElementTheme.Dark;
            if (frameworkElement.RequestedTheme == ElementTheme.Default)
            {
                isDark = App.Current.RequestedTheme == ApplicationTheme.Dark;
            }

            if (appWindow.TitleBar != null)
            {
                try
                {
                    if (isDark)
                    {
                        appWindow.TitleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
                        appWindow.TitleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
                        appWindow.TitleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
                        appWindow.TitleBar.ButtonInactiveForegroundColor = Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x80, 0x80, 0x80);
                    }
                    else
                    {
                        appWindow.TitleBar.ButtonForegroundColor = Microsoft.UI.Colors.Black;
                        appWindow.TitleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Black;
                        appWindow.TitleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.Black;
                        appWindow.TitleBar.ButtonInactiveForegroundColor = Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x80, 0x80, 0x80);
                    }

                    appWindow.TitleBar.ButtonBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(1, 0, 0, 0);
                    appWindow.TitleBar.ButtonHoverBackgroundColor = isDark
                        ? Microsoft.UI.ColorHelper.FromArgb(0x20, 0xFF, 0xFF, 0xFF)
                        : Microsoft.UI.ColorHelper.FromArgb(0x20, 0x00, 0x00, 0x00);
                    appWindow.TitleBar.ButtonPressedBackgroundColor = isDark
                        ? Microsoft.UI.ColorHelper.FromArgb(0x40, 0xFF, 0xFF, 0xFF)
                        : Microsoft.UI.ColorHelper.FromArgb(0x40, 0x00, 0x00, 0x00);
                    appWindow.TitleBar.ButtonInactiveBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(1, 0, 0, 0);
                }
                catch { }
            }
        }

        try
        {
            SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }
        catch
        {
            try { SystemBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop(); } catch { }
        }

        try
        {
            var lang = App.GetAppLanguage();
            var tService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ITranslationService>(App.Current.Services);
            _ = tService.SetLanguageAsync(lang);
        }
        catch { }

        RootFrame.Navigate(typeof(IALocalPage));
    }
}
