using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IAuthService
{
    Task<AuthUser> LoginWithGoogleAsync();
    Task<AuthUser> LoginWithGitHubAsync();
    Task<AuthUser> LinkWithGoogleAsync();
    Task<AuthUser> LinkWithGitHubAsync();
    Task<AuthUser> LoginWithEmailAndPasswordAsync(string email, string password);
    Task<AuthUser> RegisterWithEmailAndPasswordAsync(string email, string password);
    Task<AuthUser?> GetCurrentUserAsync();
    void Logout();
    bool IsAuthenticated { get; }
    string? CurrentUserUid { get; }
}
