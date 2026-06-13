using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

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
        TrayQuickAccessCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ShowQuickAccess);
        InitializeComponent();
        this.Closed += MainWindow_Closed;

        // SystemBackdrop já está definido no MainWindow.xaml

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MainPage));

        RootFrame.Loaded += RootFrame_Loaded;

        // Iniciar maximizado apenas após a janela ser ativada
        this.Activated += MainWindow_Activated;
        this.Activated += MainWindow_FocusChanged;
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        this.Activated -= MainWindow_Activated; // Executa apenas na primeira vez
        var presenter = AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
        presenter?.Maximize();
    }

    private DateTime _lastFocusCheck = DateTime.MinValue;

    private void MainWindow_FocusChanged(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            // Debounce: Evita spammar o Firestore (limita a 1 check a cada 5 segundos)
            if ((DateTime.Now - _lastFocusCheck).TotalSeconds > 5)
            {
                _lastFocusCheck = DateTime.Now;
                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAuthService>(App.Current.Services);
                        var licenseService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ILicenseService>(App.Current.Services);

                        var user = await authService.GetCurrentUserAsync();
                        if (user != null)
                        {
                            var plan = await licenseService.GetCurrentPlanAsync();
                            bool isPro = App.IsProBuild && plan.Equals("pro", StringComparison.OrdinalIgnoreCase);
                            
                            // Se o plano mudou (ex: acabou de pagar no Stripe)
                            if (App.IsProUnlocked != isPro)
                            {
                                App.IsProUnlocked = isPro;
                                DispatcherQueue?.TryEnqueue(() =>
                                {
                                    CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(isPro));
                                });
                            }
                        }
                    }
                    catch { }
                });
            }
        }
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        // Cancel the close, hide instead so the app keeps running in the background for hotkeys
        args.Handled = true;
        this.AppWindow.Hide();
    }

    public System.Windows.Input.ICommand TrayShowCommand { get; }
    public System.Windows.Input.ICommand TrayQuickAccessCommand { get; }

    // ── Quick Access Panel (singleton) ────────────────────────────────────────
    private Views.QuickAccessWindow? _quickAccessWindow;

    private void ShowQuickAccess()
    {
        try
        {
            // Toggle: se já está visível, fecha
            if (_quickAccessWindow != null && _quickAccessWindow.AppWindow.IsVisible)
            {
                _quickAccessWindow.AppWindow.Hide();
                return;
            }

            // Recria se a janela foi destruída
            if (_quickAccessWindow == null)
            {
                _quickAccessWindow = new Views.QuickAccessWindow();
                _quickAccessWindow.Closed += (s, e) => _quickAccessWindow = null;
            }

            _quickAccessWindow.ShowAtTray();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Erro ao abrir o painel de Acesso Rápido.");
        }
    }
    // ──────────────────────────────────────────────────────────────────────────

    private void ShowApp()
    {
        this.AppWindow.Show();
        var presenter = AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
        if (presenter != null && presenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Minimized)
        {
            presenter.Restore(); // Restaura caso estivesse minimizado
        }
        
        WinRT.Interop.WindowNative.GetWindowHandle(this);
        // Bring to front
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        // (If needed, use P/Invoke SetForegroundWindow, but Show() usually suffices for WinUI 3)
    }

    private async void TrayUpdate_Click(object sender, RoutedEventArgs e)
    {
        ShowApp();
        try
        {
            // Verifica se está rodando como pacote MSIX (Microsoft Store)
            var currentPackage = Windows.ApplicationModel.Package.Current;
            if (currentPackage != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://downloadsAndUpdates"));
                return;
            }
        }
        catch { }

        // Se for EXE Portable/GitHub, usa o Velopack
        try
        {
            var mgr = new Velopack.UpdateManager("https://github.com/foreigntechnologies/DesktopCommandCenter-Community");
            var newVersion = await mgr.CheckForUpdatesAsync();
            if (newVersion != null)
            {
                // Mostra um dialog no RootFrame
                CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage("UpdateAvailable"));
            }
        }
        catch { }
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
        ShowApp();
        
        if (RootFrame.Content is Microsoft.UI.Xaml.Controls.Page currentPage)
        {
            var dialog = new Views.BugReportDialog();
            dialog.XamlRoot = currentPage.XamlRoot;
            await dialog.ShowAsync();
        }
        else
        {
            // Fallback
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:suporte@foreigntechnologies.com.br?subject=Bug%20Report%20-%20DCC"));
        }
    }

    private void TrayExit_Click(object sender, RoutedEventArgs e)
    {
        // Force the app to close completely
        Microsoft.UI.Xaml.Application.Current.Exit();
    }

    private async void RootFrame_Loaded(object sender, RoutedEventArgs e)
    {
        RootFrame.Loaded -= RootFrame_Loaded; // Run only once
        
        // Verifica a licença no startup em background
        _ = System.Threading.Tasks.Task.Run(async () =>
        {
            try
            {
                var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAuthService>(App.Current.Services);
                var licenseService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ILicenseService>(App.Current.Services);

                var user = await authService.GetCurrentUserAsync();
                if (user != null)
                {
                    var plan = await licenseService.GetCurrentPlanAsync();
                    bool isPro = App.IsProBuild && plan.Equals("pro", StringComparison.OrdinalIgnoreCase);
                    App.IsProUnlocked = isPro;
                    
                    DispatcherQueue?.TryEnqueue(() =>
                    {
                        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(isPro));
                    });
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Erro ao verificar a licença inicial no startup.");
            }
        });
        
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
