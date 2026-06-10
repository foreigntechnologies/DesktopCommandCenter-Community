using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IClipboardRepository, ClipboardRepository>();
        services.AddSingleton<ITranslationService, DesktopCommandCenter.Infrastructure.Services.TranslationService>();
        services.AddSingleton<IClipboardService, DesktopCommandCenter.Infrastructure.Services.WindowsClipboardService>();
        services.AddSingleton<IAuthService, DesktopCommandCenter.Infrastructure.Services.FirebaseAuthService>();
        services.AddSingleton<DesktopCommandCenter.Application.Interfaces.IColorPickerService, DesktopCommandCenter.Infrastructure.Services.ColorPickerService>();
        services.AddSingleton<DesktopCommandCenter.Application.Interfaces.ILicenseService, DesktopCommandCenter.Infrastructure.Services.FirestoreLicenseService>();
        return services;
    }
}
