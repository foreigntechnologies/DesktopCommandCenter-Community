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
    [DllImport("shell32.dll", SetLastError = true)]
    static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

    public MainWindow()
    {
InitializeComponent();
            UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        TrayShowCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ShowApp);
        TrayQuickAccessCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ShowQuickAccess);
        
        this.Closed += MainWindow_Closed;
        try
        {
            SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }
        catch
        {
            try
            {
                SystemBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
            }
            catch { }
        }

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        
        try { SetCurrentProcessExplicitAppUserModelID("ForeignTechnologies.DCC.MainApp"); } catch { }
        var iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "DCCAppIcon.ico");
        if (System.IO.File.Exists(iconPath))
        {
            AppWindow.SetIcon(iconPath);
        }

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MainPage));

        RootFrame.Loaded += RootFrame_Loaded;
        this.Activated += MainWindow_FocusChanged;

        // Set initial title bar button colors once the frame is ready.
        // RootFrame is always a FrameworkElement so no cast needed.
        RootFrame.Loaded += (s, e) => UpdateTitleBarButtonColors();
    }

    /// <summary>
    /// Updates the caption button foreground/background colors to match the current theme.
    /// Called once on load and again whenever the theme changes.
    /// Safe to call only from the UI thread.
    /// </summary>
    public void UpdateTitleBarButtonColors()
    {
        DispatcherQueue?.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            try
            {
                if (Content is not Microsoft.UI.Xaml.FrameworkElement root) return;

                // Avoid fail-fast COM exception when updating AppWindow.TitleBar properties
                // while the window is minimized or not fully initialized.
                if (AppWindow?.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter && 
                    presenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Minimized)
                    return;

                // Resolve actual theme (light/dark)
                var isDark = root.ActualTheme == Microsoft.UI.Xaml.ElementTheme.Dark;

                var titleBar = AppWindow?.TitleBar;
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

                // Keep backgrounds transparent so Mica/Acrylic shows through
                titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonHoverBackgroundColor = isDark
                    ? Microsoft.UI.ColorHelper.FromArgb(0x20, 0xFF, 0xFF, 0xFF)
                    : Microsoft.UI.ColorHelper.FromArgb(0x20, 0x00, 0x00, 0x00);
                titleBar.ButtonPressedBackgroundColor = isDark
                    ? Microsoft.UI.ColorHelper.FromArgb(0x40, 0xFF, 0xFF, 0xFF)
                    : Microsoft.UI.ColorHelper.FromArgb(0x40, 0x00, 0x00, 0x00);
                titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;

                // Re-subscribe to ActualThemeChanged each time to avoid double-subscription
                root.ActualThemeChanged -= Root_ActualThemeChanged;
                root.ActualThemeChanged += Root_ActualThemeChanged;
            }
            catch { /* Silently ignore if AppWindow is in an invalid state */ }
        });
    }

    private void Root_ActualThemeChanged(Microsoft.UI.Xaml.FrameworkElement sender, object args)
    {
        UpdateTitleBarButtonColors();
    }

    private DateTime _lastFocusCheck = DateTime.MinValue;

    private void MainWindow_FocusChanged(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            if ((DateTime.Now - _lastFocusCheck).TotalSeconds > 5)
            {
                _lastFocusCheck = DateTime.Now;
                
                try
                {
                    var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAuthService>(App.Current.Services);
                    var licenseService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ILicenseService>(App.Current.Services);
                    var dq = this.DispatcherQueue;

                    _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        try
                        {
                            var user = await authService.GetCurrentUserAsync();
                            if (user != null)
                            {
                                var plan = await licenseService.GetCurrentPlanAsync();
                                bool isPro = plan.Equals("pro", StringComparison.OrdinalIgnoreCase);
                                
                                if (App.IsProUnlocked != isPro)
                                {
                                    App.IsProUnlocked = isPro;
                                    dq?.TryEnqueue(() =>
                                    {
                                        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Messages.LicenseChangedMessage(isPro));
                                    });
                                }
                            }
                        }
                        catch { }
                    });
                }
                catch { }
            }
        }
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        // Ao invés de fechar (X), oculta o app para a bandeja do sistema
        args.Handled = true;
        this.AppWindow.Hide();
    }

    public System.Windows.Input.ICommand TrayShowCommand { get; }
    public System.Windows.Input.ICommand TrayQuickAccessCommand { get; }

    // â”€â”€ Quick Access Panel (singleton) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    private Views.QuickAccessWindow? _quickAccessWindow;
    private DesktopCommandCenter.Application.Interfaces.ITerminalService? _terminalService;
    private bool _isTerminalOpen = false;

    private void ToggleTerminal()
    {
        DispatcherQueue.TryEnqueue(() => 
        {
            _isTerminalOpen = !_isTerminalOpen;
            if (_isTerminalOpen)
            {
                ShowApp();
                TerminalOverlay.Visibility = Visibility.Visible;
                TerminalSlideUpAnimation.Begin();
                TerminalWebView.Focus(FocusState.Programmatic);
            }
            else
            {
                TerminalSlideDownAnimation.Begin();
            }
        });
    }

    private void TerminalSlideDownAnimation_Completed(object sender, object e)
    {
        TerminalOverlay.Visibility = Visibility.Collapsed;
    }

    private void CloseTerminal_Click(object sender, RoutedEventArgs e)
    {
        if (_isTerminalOpen) ToggleTerminal();
    }

    private class QuickCommandItem
    {
        public string Title { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
    }

    private System.Collections.Generic.List<QuickCommandItem> _quickCommands = new();
    
    private void LoadQuickCommands()
    {
        try
        {
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC", "dcc_quick_commands.json");
            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                var items = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<QuickCommandItem>>(json);
                if (items != null) _quickCommands = items;
            }
            else
            {
                _quickCommands = new System.Collections.Generic.List<QuickCommandItem>
                {
                    new QuickCommandItem { Title = "Ping Google", Command = "ping google.com" },
                    new QuickCommandItem { Title = "IP Config", Command = "ipconfig" }
                };
                SaveQuickCommands();
            }
            RenderQuickCommands();
        }
        catch { }
    }

    private void SaveQuickCommands()
    {
        try
        {
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC", "dcc_quick_commands.json");
            var json = System.Text.Json.JsonSerializer.Serialize(_quickCommands);
            System.IO.File.WriteAllText(path, json);
        }
        catch { }
    }

    private void RenderQuickCommands()
    {
        QuickCommandsPanel.Children.Clear();
        foreach (var cmd in _quickCommands)
        {
            var btn = new Microsoft.UI.Xaml.Controls.Button
            {
                Content = cmd.Title,
                FontSize = 12,
                Tag = cmd.Command,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent),
                BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources["CardBorderBrush"]
            };
            btn.Click += async (s, e) =>
            {
                if (_terminalService != null && s is Microsoft.UI.Xaml.Controls.Button b && b.Tag is string commandText)
                {
                    await _terminalService.WriteInputAsync(commandText + "\r\n");
                }
            };
            QuickCommandsPanel.Children.Add(btn);
        }

        var addBtn = new Microsoft.UI.Xaml.Controls.Button
        {
            Content = "+ Novo",
            FontSize = 12,
            Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent),
            BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources["CardBorderBrush"]
        };
        addBtn.Click += AddQuickCommand_Click;
        QuickCommandsPanel.Children.Add(addBtn);
    }

    private async void AddQuickCommand_Click(object sender, RoutedEventArgs e)
    {
        var titleBox = new Microsoft.UI.Xaml.Controls.TextBox { PlaceholderText = "Título (ex: Ping)", Margin = new Thickness(0, 0, 0, 8) };
        var cmdBox = new Microsoft.UI.Xaml.Controls.TextBox { PlaceholderText = "Comando (ex: ping google.com)" };
        var panel = new Microsoft.UI.Xaml.Controls.StackPanel();
        panel.Children.Add(titleBox);
        panel.Children.Add(cmdBox);

        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            Title = "Novo Comando Rápido",
            Content = panel,
            PrimaryButtonText = "Salvar",
            CloseButtonText = "Cancelar",
            XamlRoot = this.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(titleBox.Text) && !string.IsNullOrWhiteSpace(cmdBox.Text))
        {
            _quickCommands.Add(new QuickCommandItem { Title = titleBox.Text, Command = cmdBox.Text });
            SaveQuickCommands();
            RenderQuickCommands();
        }
    }

    private async void CmbShellSelector_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        if (_terminalService == null) return;
        
        if (CmbShellSelector.SelectedItem is Microsoft.UI.Xaml.Controls.ComboBoxItem item && item.Tag is string shell)
        {
            try
            {
                _terminalService.Stop();
                await _terminalService.StartAsync(shell, 80, 24);
                // Trigger an initial resize logic if WebView is loaded
                if (TerminalWebView.CoreWebView2 != null)
                {
                    TerminalWebView.CoreWebView2.PostWebMessageAsJson("{\"type\":\"resize_trigger\"}");
                }
            }
            catch { }
        }
    }

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
    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

        if (!App.HasAppLanguageCached() && RootFrame.Content is Microsoft.UI.Xaml.Controls.Page currentPage)
        {
            var combo = new Microsoft.UI.Xaml.Controls.ComboBox
            {
                Items = { "Português - Brasil", "English", "Español" },
                SelectedIndex = 0,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch
            };
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Select Language / Selecione o Idioma / Seleccionar Idioma",
                Content = combo,
                PrimaryButtonText = "OK",
                XamlRoot = currentPage.XamlRoot
            };

            await dialog.ShowAsync();
            string lang = combo.SelectedIndex switch
            {
                1 => "en-US",
                2 => "es-ES",
                _ => "pt-BR"
            };
            App.SaveAppLanguage(lang);
            var tService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ITranslationService>(App.Current.Services);
            _ = tService.SetLanguageAsync(lang);
        }
        
        // Salva uma referência para usar dentro do Task.Run com segurança
        var currentServices = App.Current.Services;
        
        // Verifica a licença no startup em background
        _ = System.Threading.Tasks.Task.Run(async () =>
        {
            try
            {
                var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAuthService>(currentServices);
                var licenseService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ILicenseService>(currentServices);

                var user = await authService.GetCurrentUserAsync();
                if (user != null)
                {
                    var plan = await licenseService.GetCurrentPlanAsync();
                    bool isPro = plan.Equals("pro", StringComparison.OrdinalIgnoreCase);
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

        try
        {
            await TerminalWebView.EnsureCoreWebView2Async();
            var terminalPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Assets", "Terminal", "terminal.html");
            TerminalWebView.CoreWebView2.Navigate(new Uri(terminalPath).AbsoluteUri);

            TerminalWebView.CoreWebView2.WebMessageReceived += async (s, args) =>
            {
                try
                {
                    var json = args.TryGetWebMessageAsString();
                    var msg = System.Text.Json.JsonDocument.Parse(json).RootElement;
                    var type = msg.GetProperty("type").GetString();
                    if (type == "input")
                    {
                        var data = msg.GetProperty("data").GetString();
                        if (_terminalService != null && data != null)
                            await _terminalService.WriteInputAsync(data);
                    }
                    else if (type == "resize")
                    {
                        int cols = msg.GetProperty("cols").GetInt32();
                        int rows = msg.GetProperty("rows").GetInt32();
                        _terminalService?.Resize(cols, rows);
                    }
                }
                catch { }
            };

            var hotkeyService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IHotkeyService>(App.Current.Services);
            // Ctrl + ' (Oem3 is 0xC0 = 192)
            hotkeyService.RegisterHotkey(2, 192, ToggleTerminal);

            _terminalService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DesktopCommandCenter.Application.Interfaces.ITerminalService>(App.Current.Services);
            _terminalService.OutputDataReceived += (s, text) =>
            {
                DispatcherQueue?.TryEnqueue(() =>
                {
                    try
                    {
                        var escaped = System.Text.Json.JsonSerializer.Serialize(new { type = "output", data = text });
                        TerminalWebView.CoreWebView2.PostWebMessageAsJson(escaped);
                    }
                    catch { }
                });
            };
            
            LoadQuickCommands();
            
            // Note: StartAsync is now triggered by CmbShellSelector_SelectionChanged as it defaults to index 0
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Erro ao inicializar o Terminal (FutureShell).");
        }
    }

        private void UpdateTranslations()
        {
            TrayUpdateAvailableElement.Text = Helpers.LocalizationHelper.Instance.GetString("Tray_UpdateAvailable");
            TrayQuickAccessElement.Text = Helpers.LocalizationHelper.Instance.GetString("Tray_QuickAccess");
            TraySettingsElement.Text = Helpers.LocalizationHelper.Instance.GetString("Tray_Settings");
            TrayDocumentationElement.Text = Helpers.LocalizationHelper.Instance.GetString("Tray_Documentation");
            TrayReportBugElement.Text = Helpers.LocalizationHelper.Instance.GetString("Tray_ReportBug");
            TrayExitElement.Text = Helpers.LocalizationHelper.Instance.GetString("Tray_Exit");
            if (MainWindowNewCommandElement.Content is string || MainWindowNewCommandElement.Content == null) MainWindowNewCommandElement.Content = Helpers.LocalizationHelper.Instance.GetString("MainWindow_NewCommand");
        }
}

