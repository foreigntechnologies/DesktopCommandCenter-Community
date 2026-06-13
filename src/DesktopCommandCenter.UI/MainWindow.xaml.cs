using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.Mvvm.Messaging;

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
        TrayShowCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ShowApp);
        InitializeComponent();
        this.Closed += MainWindow_Closed;

        // Habilitar efeito Mica (Translúcido do Windows 11)
        SystemBackdrop = new MicaBackdrop();
        
        // Centralizar e redimensionar a janela como uma barra lateral lateral
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");

        if (Content is FrameworkElement rootElement)
        {
            // TitleBar control automatically updates with theme, no need to manually set AppWindow colors
        }

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MainPage));

        RootFrame.Loaded += RootFrame_Loaded;

        // Iniciar maximizado
        var presenter = AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
        presenter?.Maximize();
    }


    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        // Cancel the close, hide instead so the app keeps running in the background for hotkeys
        args.Handled = true;
        this.AppWindow.Hide();
    }

    public System.Windows.Input.ICommand TrayShowCommand { get; }

    private void ShowApp()
    {
        this.AppWindow.Show();
        WinRT.Interop.WindowNative.GetWindowHandle(this);
        // Bring to front
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        // (If needed, use P/Invoke SetForegroundWindow, but Show() usually suffices for WinUI 3)
    }

    private void TrayUpdate_Click(object sender, RoutedEventArgs e)
    {
        ShowApp();
        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("Settings"));
    }

    private void TrayShow_Click(object sender, RoutedEventArgs e)
    {
        ShowApp();
    }

    private void TraySettings_Click(object sender, RoutedEventArgs e)
    {
        ShowApp();
        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("Settings"));
    }

    private async void TrayDocs_Click(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/foreigntechnologies/DesktopCommandCenter-Community"));
    }

    private async void TrayBug_Click(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:suporte@foreigntechnologies.com.br?subject=Bug%20Report%20-%20DCC"));
    }

    private void TrayExit_Click(object sender, RoutedEventArgs e)
    {
        // Force the app to close completely
        Microsoft.UI.Xaml.Application.Current.Exit();
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
