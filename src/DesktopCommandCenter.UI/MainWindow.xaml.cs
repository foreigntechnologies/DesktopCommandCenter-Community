using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DesktopCommandCenter.UI;

/// <summary>
/// The application window. This hosts a Frame that displays pages. Add your
/// UI and logic to MainPage.xaml / MainPage.xaml.cs instead of here so you
/// can use Page features such as navigation events and the Loaded lifecycle.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Closed += MainWindow_Closed;

        // Habilitar efeito Mica (Translúcido do Windows 11)
        SystemBackdrop = new MicaBackdrop();
        
        // Centralizar e redimensionar a janela como uma barra lateral lateral
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

        ExtendsContentIntoTitleBar = true;

        AppWindow.SetIcon("Assets/AppIcon.ico");

        if (Content is FrameworkElement rootElement)
        {
            // TitleBar control automatically updates with theme, no need to manually set AppWindow colors
        }

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MainPage));

        RootFrame.Loaded += RootFrame_Loaded;
    }


    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        // Cancel the close, hide instead so the app keeps running in the background for hotkeys
        args.Handled = true;
        this.AppWindow.Hide();
    }

    private async void RootFrame_Loaded(object sender, RoutedEventArgs e)
    {
        RootFrame.Loaded -= RootFrame_Loaded; // Run only once
        
        bool firstRun = false;
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_theme.txt");
            
            bool hasPackagedSettings = false;
            try
            {
                hasPackagedSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("AppTheme");
            }
            catch { }

            if (!System.IO.File.Exists(filePath) && !hasPackagedSettings)
            {
                firstRun = true;
            }
        }
        catch { }

        if (firstRun)
        {
            var dialog = new Views.OobeDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }
        else
        {
            string themeStr = App.GetTheme();
            App.ApplyTheme(themeStr);
        }
    }
}
