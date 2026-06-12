using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using DesktopCommandCenter.Infrastructure.Services;

namespace DesktopCommandCenter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IClipboardRepository, ClipboardRepository>();
        services.AddScoped<IPromptRepository, PromptRepository>();
        services.AddSingleton<ITranslationService, TranslationService>();
        services.AddSingleton<IClipboardService, WindowsClipboardService>();
        services.AddSingleton<IAuthService, FirebaseAuthService>();
        services.AddSingleton<IColorPickerService, ColorPickerService>();
        services.AddSingleton<ILicenseService, FirestoreLicenseService>();
        services.AddSingleton<IHotkeyService, GlobalHotkeyService>();
        services.AddSingleton<IHotkeyConfigManager, HotkeyConfigManager>();
        
        services.AddSingleton<IIAAgentService, SemanticKernelAgentService>();
        services.AddTransient<IWhisperTranscriptionService, WhisperTranscriptionService>();
        return services;
    }
}
