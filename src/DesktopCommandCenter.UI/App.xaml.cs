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
                mainWindow.AppWindow.TitleBar.ButtonForegroundColor = isDark ? Microsoft.UI.Colors.White : Microsoft.UI.Colors.Black;
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
    
    public static bool IsProUnlocked { get; set; } = false;

    public App()
    {
        // Inicializa o Velopack no início da aplicação para gerenciar atalhos e atualizações
        Velopack.VelopackApp.Build().Run();

        IsProUnlocked = false;

        AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
        {
            try
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "dcc_firstchance.txt"), args.Exception.ToString() + "\n\n");
            }
            catch { }
        };

        Services = ConfigureServices();
        InitializeComponent();
        
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
        
        // Start Smart Clipboard Monitoring
        var clipboardService = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IClipboardService>();
        clipboardService.StartMonitoring();

        // Register Dynamic Hotkeys
        RegisterAllHotkeys();
        var configManager = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IHotkeyConfigManager>();
        configManager.ConfigsChanged += (s, e) => RegisterAllHotkeys();
        
        Log.Information("DCC Inicializado com sucesso.");
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
        MainWindow = new MainWindow();
        MainWindow.Activate();
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

