using DesktopCommandCenter.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Services;

public class DummyAuthService : IAuthService
{
    public bool IsAuthenticated => false;
    public string? CurrentUserUid => null;

    public Task<AuthUser> LoginWithGitHubAsync() => Task.FromResult(new AuthUser { Uid = "dummy123", Email = "community@user.com", IdToken = "dummy-token" });
    public Task<AuthUser> LoginWithGoogleAsync() => Task.FromResult(new AuthUser { Uid = "dummy123", Email = "community@user.com", IdToken = "dummy-token" });
    public Task<AuthUser> LoginWithMicrosoftAsync() => Task.FromResult(new AuthUser { Uid = "dummy123", Email = "community@user.com", IdToken = "dummy-token" });
    public Task<AuthUser> LinkWithGitHubAsync() => Task.FromResult(new AuthUser { Uid = "dummy123", Email = "community@user.com", IdToken = "dummy-token" });
    public Task<AuthUser> LinkWithGoogleAsync() => Task.FromResult(new AuthUser { Uid = "dummy123", Email = "community@user.com", IdToken = "dummy-token" });
    public Task<AuthUser> LinkWithMicrosoftAsync() => Task.FromResult(new AuthUser { Uid = "dummy123", Email = "community@user.com", IdToken = "dummy-token" });
    public Task<AuthUser> LoginWithEmailAndPasswordAsync(string email, string password) => Task.FromResult(new AuthUser { Uid = "dummy123", Email = email, IdToken = "dummy-token" });
    public Task<AuthUser> RegisterWithEmailAndPasswordAsync(string email, string password) => Task.FromResult(new AuthUser { Uid = "dummy", Email = email });
    public Task<AuthUser?> GetCurrentUserAsync(bool forceRefresh = false) => Task.FromResult<AuthUser?>(null);
    public void Logout() { }
    public void CancelLogin() { }
}

public class DummyLicenseService : ILicenseService
{
    public Task<bool> IsProUserAsync() => Task.FromResult(false);
    public Task<string> GetCurrentPlanAsync() => Task.FromResult("free");
}

public class DummyIAAgentService : IIAAgentService
{
#pragma warning disable CS1998
    public async IAsyncEnumerable<string> SendMessageStreamAsync(string prompt, string? imagePath = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return "Agent feature is only available in the PRO version.";
    }

    public async IAsyncEnumerable<string> SendAudioStreamAsync(string audioFilePath, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return "Transcription & Agent features are only available in the PRO version.";
    }
#pragma warning restore CS1998

    public void ClearHistory() { }

    public Task<List<string>> GetAvailableModelsAsync() => Task.FromResult(new List<string> { "PRO Feature Only" });
    
    public void SetModel(string modelId) { }

    public Task PullModelAsync(string modelName, System.IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

public class DummyWhisperTranscriptionService : IWhisperTranscriptionService
{
    public void StartRecording() { }
    public Task<string> StopRecordingAndTranscribeAsync(CancellationToken cancellationToken = default) => Task.FromResult("Transcription feature is only available in the PRO version.");
    public Task<string> TranscribeFileAsync(string filePath, CancellationToken cancellationToken = default) => Task.FromResult("Transcription feature is only available in the PRO version.");
}
