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
    private Window? _window;
    
    public new static App Current => (App)Microsoft.UI.Xaml.Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
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
                    // Uses WeakReferenceMessenger to notify MainPage
                    WeakReferenceMessenger.Default.Send(new Messages.NavigateMessage(config.ActionId));
                }
            });
        }
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
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
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.NotesViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.AuthViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.ClipboardViewModel>();
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.SettingsViewModel>();
        services.AddSingleton<DesktopCommandCenter.UI.ViewModels.ColorPickerViewModel>();
        
        // Configurar MediatR / Application
        services.AddApplication();
        services.AddInfrastructure();
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
            
        return services.BuildServiceProvider();
    }
}

