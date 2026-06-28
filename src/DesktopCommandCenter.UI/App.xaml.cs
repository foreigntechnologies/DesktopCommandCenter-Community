using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using DesktopCommandCenter.Infrastructure.Data;
using DesktopCommandCenter.Infrastructure;

using Serilog;
using DesktopCommandCenter.Application;
using CommunityToolkit.Mvvm.Messaging;

namespace DesktopCommandCenter.UI;

public partial class App : Microsoft.UI.Xaml.Application
{
    public Window? MainWindow { get; private set; }
    
    public new static App Current => (App)Microsoft.UI.Xaml.Application.Current;

    public static string GetTheme()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_theme.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }

        try
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("AppTheme", out object? themeObj) && themeObj is string themeStr)
            {
                return themeStr;
            }
        }
        catch { }

        return "Default";
    }

    public static void SaveTheme(string themeStr)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_theme.txt");
            System.IO.File.WriteAllText(filePath, themeStr);
        }
        catch { }

        try
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppTheme"] = themeStr;
        }
        catch { }
    }

    public static void ApplyTheme(string themeStr)
    {
        var mainWindow = App.Current.MainWindow;
        if (mainWindow?.Content is Microsoft.UI.Xaml.FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = themeStr switch
            {
                "Light" => Microsoft.UI.Xaml.ElementTheme.Light,
                "Dark" => Microsoft.UI.Xaml.ElementTheme.Dark,
                _ => Microsoft.UI.Xaml.ElementTheme.Default
            };

            var isDark = frameworkElement.RequestedTheme == Microsoft.UI.Xaml.ElementTheme.Dark;
            if (frameworkElement.RequestedTheme == Microsoft.UI.Xaml.ElementTheme.Default)
            {
                isDark = App.Current.RequestedTheme == Microsoft.UI.Xaml.ApplicationTheme.Dark;
            }

            if (mainWindow.AppWindow?.TitleBar != null)
            {
                try
                {
                    mainWindow.AppWindow.TitleBar.ButtonForegroundColor = isDark ? Microsoft.UI.Colors.White : Microsoft.UI.Colors.Black;
                }
                catch
                {
                    // Ignora exceções COM/ArgumentException que ocorrem ao maximizar no WinUI 3
                }
            }
        }
    }

    public static string GetTimeFormat()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_time_format.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }
        return "HH:mm";
    }

    public static void SaveTimeFormat(string format)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_time_format.txt");
            System.IO.File.WriteAllText(filePath, format);
        }
        catch { }
    }

    public static string GetDateFormat()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_date_format.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }
        return "dddd, dd MMMM yyyy";
    }

    public static void SaveDateFormat(string format)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_date_format.txt");
            System.IO.File.WriteAllText(filePath, format);
        }
        catch { }
    }

    public IServiceProvider Services { get; }

    public static bool IsProBuild => System.IO.File.Exists(System.IO.Path.Combine(System.AppContext.BaseDirectory, "DesktopCommandCenter.ProFeatures.dll"));
    
    private static bool _isProUnlocked = false;
    public static bool IsProUnlocked
    {
        get => _isProUnlocked;
        set
        {
            if (_isProUnlocked != value)
            {
                _isProUnlocked = value;
                SaveProCached(value);
            }
        }
    }

    public static string GetAIAgentProvider()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_ai_provider.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }
        return "Ollama";
    }

    public static void SaveAIAgentProvider(string provider)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_ai_provider.txt");
            System.IO.File.WriteAllText(filePath, provider);
        }
        catch { }
    }

    public static string GetAIAgentApiKey()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_ai_apikey.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }
        return string.Empty;
    }

    public static void SaveAIAgentApiKey(string apiKey)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_ai_apikey.txt");
            System.IO.File.WriteAllText(filePath, apiKey);
        }
        catch { }
    }

    public static string GetOpenAIApiKey()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_openai_apikey.txt");
            if (System.IO.File.Exists(filePath)) return System.IO.File.ReadAllText(filePath).Trim();
        }
        catch { }
        return string.Empty;
    }

    public static void SaveOpenAIApiKey(string apiKey)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_openai_apikey.txt");
            System.IO.File.WriteAllText(filePath, apiKey);
        }
        catch { }
    }

    public static string GetGeminiApiKey()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_gemini_apikey.txt");
            if (System.IO.File.Exists(filePath)) return System.IO.File.ReadAllText(filePath).Trim();
        }
        catch { }
        return string.Empty;
    }

    public static void SaveGeminiApiKey(string apiKey)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_gemini_apikey.txt");
            System.IO.File.WriteAllText(filePath, apiKey);
        }
        catch { }
    }

    public static string GetClaudeApiKey()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_claude_apikey.txt");
            if (System.IO.File.Exists(filePath)) return System.IO.File.ReadAllText(filePath).Trim();
        }
        catch { }
        return string.Empty;
    }

    public static void SaveClaudeApiKey(string apiKey)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_claude_apikey.txt");
            System.IO.File.WriteAllText(filePath, apiKey);
        }
        catch { }
    }

    public static string GetAIAgentModel()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_ai_model.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }
        return string.Empty;
    }

    public static void SaveAIAgentModel(string model)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_ai_model.txt");
            System.IO.File.WriteAllText(filePath, model);
        }
        catch { }
    }

    public static bool GetProCached()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_pro_cached.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim() == "true";
            }
        }
        catch { }
        return false;
    }

    public static void SaveProCached(bool isPro)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_pro_cached.txt");
            System.IO.File.WriteAllText(filePath, isPro ? "true" : "false");
        }
        catch { }
    }

    public static string GetCachedEmail()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_cached_email.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }
        return string.Empty;
    }

    public static void SaveCachedEmail(string email)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_cached_email.txt");
            System.IO.File.WriteAllText(filePath, email);
        }
        catch { }
    }

    public static bool HasAppLanguageCached()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_app_language.txt");
            return System.IO.File.Exists(filePath);
        }
        catch { return false; }
    }

    public static string GetAppLanguage()
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = System.IO.Path.Combine(dir, "dcc_app_language.txt");
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
        }
        catch { }
        
        // Fallback to System Language
        var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        if (culture.StartsWith("pt")) return "pt-BR";
        if (culture.StartsWith("es")) return "es-ES";
        return "en-US";
    }

    public static void SaveAppLanguage(string lang)
    {
        try
        {
            var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, "dcc_app_language.txt");
            System.IO.File.WriteAllText(filePath, lang);
        }
        catch { }
    }

    public App()
    {
        // Intercepta qualquer exceção grave não tratada
        DesktopCommandCenter.UI.Services.CrashLogger.Initialize();

        // Inicializa o Velopack no início da aplicação para gerenciar atalhos e atualizações
        Velopack.VelopackApp.Build().Run();

        _isProUnlocked = GetProCached();

        Services = ConfigureServices();
        InitializeComponent();
        
        EnsureFutureShellShortcuts();
        
        Log.Information("DCC Inicializado com sucesso.");
    }

    private void EnsureFutureShellShortcuts()
    {
        try
        {
            var startMenu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Desktop Command Center");
            Directory.CreateDirectory(startMenu);
            var startMenuLink = Path.Combine(startMenu, "FutureShell.lnk");
            
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var desktopLink = Path.Combine(desktop, "FutureShell.lnk");

            CreateShortcut(startMenuLink);
            CreateShortcut(desktopLink);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Falha ao criar atalhos do FutureShell.");
        }
    }

    private void CreateShortcut(string shortcutPath)
    {
        if (System.IO.File.Exists(shortcutPath)) return;
        
        Type? t = Type.GetTypeFromProgID("WScript.Shell");
        if (t != null)
        {
            dynamic? shell = Activator.CreateInstance(t);
            if (shell != null)
            {
                var shortcut = shell.CreateShortcut(shortcutPath);
                
                string exePath = Environment.ProcessPath ?? "";
                string arguments = "--futureshell";
                
                if (!string.IsNullOrEmpty(exePath))
                {
                    string parentDir = Path.GetDirectoryName(Path.GetDirectoryName(exePath)) ?? "";
                    string updateExe = Path.Combine(parentDir, "Update.exe");
                    if (File.Exists(updateExe))
                    {
                        shortcut.TargetPath = updateExe;
                        arguments = $"--processStart \"{Path.GetFileName(exePath)}\" --processStartArgs \"--futureshell\"";
                    }
                    else
                    {
                        shortcut.TargetPath = exePath;
                    }
                }
                
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = AppContext.BaseDirectory;
                shortcut.IconLocation = Path.Combine(AppContext.BaseDirectory, "Assets", "FutureShell-IconSemTexto.ico");
                shortcut.Save();
            }
        }
    }

    private void RegisterAllHotkeys()
    {
        var hotkeyService = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IHotkeyService>();
        var configManager = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IHotkeyConfigManager>();

        hotkeyService.UnregisterAll();

        foreach (var config in configManager.GetAllConfigs())
        {
            if (config.VirtualKey == 0) continue; // Hotkey not configured

            hotkeyService.RegisterHotkey((int)config.Modifiers, (int)config.VirtualKey, () =>
            {
                if (config.ActionId == "ColorPicker")
                {
                    var colorVm = Services.GetRequiredService<DesktopCommandCenter.UI.ViewModels.ColorPickerViewModel>();
                    colorVm.TogglePicking();
                }
                else
                {
                    App.Current.MainWindow?.AppWindow.Show();
                    // Uses WeakReferenceMessenger to notify MainPage
                    WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage(config.ActionId));
                }
            });
        }
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var cmdArgs = Environment.GetCommandLineArgs();
        bool isFutureShell = false;
        foreach (var arg in cmdArgs)
        {
            if (arg.Equals("--futureshell", StringComparison.OrdinalIgnoreCase))
                isFutureShell = true;
        }

        if (isFutureShell)
        {
            MainWindow = new Views.FutureShellWindow();
            MainWindow.Activate();
            return;
        }

        MainWindow = new MainWindow();
        MainWindow.Activate();

        var dispatcherQueue = MainWindow.DispatcherQueue;

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                // Ensure Database is created
                var dbContext = Services.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS Prompts (
                        Id TEXT PRIMARY KEY,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NULL,
                        Title TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        Content TEXT NOT NULL
                    );");
                // Start Automation Engine
                var automationEngine = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IAutomationEngine>();
                automationEngine.Start();

                // Register Dynamic Hotkeys and UI-thread services
                dispatcherQueue?.TryEnqueue(() =>
                {
                    // Start Smart Clipboard Monitoring
                    var clipboardService = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IClipboardService>();
                    clipboardService.StartMonitoring();

                    RegisterAllHotkeys();
                    var configManager = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IHotkeyConfigManager>();
                    configManager.ConfigsChanged += (s, e) => RegisterAllHotkeys();
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro na inicialização em background");
            }
        });
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
        var dbPath = Path.Combine(appDataPath, "dcc_community.db");
        var logPath = Path.Combine(appDataPath, "logs", "dcc_log-.txt");
        
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        
        // Configurar Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, shared: true)
            .CreateLogger();

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.NotesViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.AuthViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.ClipboardViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.SettingsViewModel>();
        services.AddSingleton<DesktopCommandCenter.UI.ViewModels.ColorPickerViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.CalculadoraViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.AwakeViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.AlwaysOnTopViewModel>();
        
        // Fase 4: Módulos "Em Breve"
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.TemporizadorViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.CapturaViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.TradutorViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.IALocalViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.PesquisaUniversalViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.PromptsViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.AutomacoesViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.CliCommandsViewModel>();
        
        // Configurar MediatR / Application
        services.AddApplication();
        services.AddInfrastructure();
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
            
        return services.BuildServiceProvider();
    }
}

