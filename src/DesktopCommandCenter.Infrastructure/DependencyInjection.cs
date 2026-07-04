using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using DesktopCommandCenter.Infrastructure.Services;

namespace DesktopCommandCenter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<INoteRepository, NoteRepository>();
        services.AddTransient<IClipboardRepository, ClipboardRepository>();
        services.AddTransient<IPromptRepository, PromptRepository>();
        services.AddSingleton<ITranslationService, TranslationService>();
        services.AddSingleton<IClipboardService, WindowsClipboardService>();
        services.AddSingleton<IColorPickerService, ColorPickerService>();
        services.AddSingleton<IHotkeyService, GlobalHotkeyService>();
        services.AddSingleton<IHotkeyConfigManager, HotkeyConfigManager>();
        services.AddSingleton<IAutomationEngine, AutomationEngine>();
        services.AddSingleton<IProcessService, ProcessService>();
        services.AddTransient<ITerminalService, ConPTYService>();
        services.AddTransient<IDeepCleanService, DeepCleanService>();
        services.AddSingleton<IWebSearchService, DuckDuckGoSearchService>();
        
        // Load PRO Features if available
        var proAssemblyPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "DesktopCommandCenter.ProFeatures.dll");
        if (System.IO.File.Exists(proAssemblyPath))
        {
            try
            {
                var assembly = System.Reflection.Assembly.LoadFrom(proAssemblyPath);
                var bootstrapperType = assembly.GetType("DesktopCommandCenter.ProFeatures.ProBootstrapper");
                if (bootstrapperType != null)
                {
                    var method = bootstrapperType.GetMethod("RegisterServices");
                    method?.Invoke(null, new object[] { services });
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load ProFeatures: {ex.Message}");
            }
        }
        else
        {
            // Register Dummy Services for Community Edition
            services.AddSingleton<IAuthService, DummyAuthService>();
            services.AddSingleton<ILicenseService, DummyLicenseService>();
            services.AddSingleton<IIAAgentService, DummyIAAgentService>();
            services.AddTransient<IWhisperTranscriptionService, DummyWhisperTranscriptionService>();
        }

        return services;
    }
}
