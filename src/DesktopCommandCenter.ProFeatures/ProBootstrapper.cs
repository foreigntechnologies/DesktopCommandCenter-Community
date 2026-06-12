using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.ProFeatures.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.ProFeatures;

public static class ProBootstrapper
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAuthService, FirebaseAuthService>();
        services.AddSingleton<ILicenseService, FirestoreLicenseService>();
        services.AddSingleton<IIAAgentService, SemanticKernelAgentService>();
        services.AddTransient<IWhisperTranscriptionService, WhisperTranscriptionService>();
    }
}
