using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using DesktopCommandCenter.Infrastructure.Data;
using DesktopCommandCenter.Infrastructure;

using Serilog;
using DesktopCommandCenter.Application;

namespace DesktopCommandCenter.UI;

public partial class App : Microsoft.UI.Xaml.Application
{
    private Window? _window;
    
    public new static App Current => (App)Microsoft.UI.Xaml.Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
        
        // Ensure Database is created
        var dbContext = Services.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
        
        // Iniciar Monitoramento Smart Clipboard
        var clipboardService = Services.GetRequiredService<DesktopCommandCenter.Application.Interfaces.IClipboardService>();
        clipboardService.StartMonitoring();
        
        Log.Information("DCC Inicializado com sucesso.");
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
        services.AddTransient<DesktopCommandCenter.UI.ViewModels.ColorPickerViewModel>();
        
        // Configurar MediatR / Application
        services.AddApplication();
        services.AddInfrastructure();
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
            
        return services.BuildServiceProvider();
    }
}

