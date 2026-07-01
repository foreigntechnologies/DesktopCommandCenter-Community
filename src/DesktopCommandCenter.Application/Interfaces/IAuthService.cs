using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IAuthService
{
    Task<AuthUser> LoginWithGoogleAsync();
    Task<AuthUser> LoginWithGitHubAsync();
    Task<AuthUser> LoginWithMicrosoftAsync();
    Task<AuthUser> LinkWithGoogleAsync();
    Task<AuthUser> LinkWithGitHubAsync();
    Task<AuthUser> LinkWithMicrosoftAsync();
    Task<AuthUser> LoginWithEmailAndPasswordAsync(string email, string password);
    Task<AuthUser> RegisterWithEmailAndPasswordAsync(string email, string password);
    Task<AuthUser?> GetCurrentUserAsync(bool forceRefresh = false);
    void Logout();
    void CancelLogin();
    bool IsAuthenticated { get; }
    string? CurrentUserUid { get; }
}
