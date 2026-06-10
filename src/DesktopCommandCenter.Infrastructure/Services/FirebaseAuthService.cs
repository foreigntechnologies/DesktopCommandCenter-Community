using DesktopCommandCenter.Application.Interfaces;
using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Services;

public class FirebaseAuthService : IAuthService
{
    private readonly FirebaseAuthClient _client;

    public bool IsAuthenticated => _client.User != null;
    public string? CurrentUserUid => _client.User?.Uid;

    public FirebaseAuthService()
    {
        // A API Key e o Domínio devem ser movidos para uma configuração externa ou appsettings.json.
        var config = new FirebaseAuthConfig
        {
            ApiKey = "SUA_WEB_API_KEY_AQUI", // A Service Account não entra aqui, precisa da Web API Key pública
            AuthDomain = "desktop-command-center-windows.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new GoogleProvider(),
                new GithubProvider()
            }
        };

        _client = new FirebaseAuthClient(config);
    }

    public Task<AuthUser> LoginWithGoogleAsync()
    {
        throw new System.NotImplementedException("Fluxo OAuth para Desktop (Google) requer injeção do token. Próxima etapa.");
    }

    public Task<AuthUser> LoginWithGitHubAsync()
    {
        throw new System.NotImplementedException("Fluxo OAuth para Desktop (GitHub) requer injeção do token. Próxima etapa.");
    }

    public async Task<AuthUser?> GetCurrentUserAsync()
    {
        var user = _client.User;
        if (user == null) return null;

        var token = await user.GetIdTokenAsync();
        return new AuthUser
        {
            Uid = user.Uid,
            Email = user.Info.Email ?? "",
            IdToken = token
        };
    }

    public void Logout()
    {
        _client.SignOut();
    }
}
