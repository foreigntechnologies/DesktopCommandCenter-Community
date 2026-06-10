using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IAuthService
{
    Task<AuthUser> LoginWithGoogleAsync();
    Task<AuthUser> LoginWithGitHubAsync();
    Task<AuthUser?> GetCurrentUserAsync();
    void Logout();
    bool IsAuthenticated { get; }
    string? CurrentUserUid { get; }
}
