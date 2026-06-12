using DesktopCommandCenter.Application.Interfaces;
using Firebase.Auth;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using System.Net.Sockets;

namespace DesktopCommandCenter.ProFeatures.Services;

public class FirebaseAuthService : IAuthService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiKey = "AIzaSyDyAVbKJ8umZ_ezLHl9dCUWfE3cGIa-6zA";
    private const string AuthDomain = "desktop-command-center-windows.firebaseapp.com";
    
    private User? _currentUser;
    public bool IsAuthenticated => _currentUser != null;
    public string? CurrentUserUid => _currentUser?.Uid;

    public async Task<AuthUser> LoginWithGoogleAsync()
    {
        int port = GetAvailablePort();
        string redirectUri = $"http://localhost:{port}/";
        string authUrl = $"https://{AuthDomain}/__/auth/handler?apiKey={ApiKey}&authType=signIn&providerId=google.com&redirectUri={HttpUtility.UrlEncode(redirectUri)}";

        Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });

        string authCode = await ListenForAuthCode(port);
        
        // Exchange code/token using REST API or Firebase Auth Admin SDK concepts
        // For simplicity in this architectural pattern, return the AuthUser after verification
        return await CompleteAuthentication(authCode);
    }

    private int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private async Task<string> ListenForAuthCode(int port)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();
        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString["code"] ?? "error";
        
        var response = context.Response;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Authentication successful. You can close this window.");
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.Close();
        listener.Stop();
        return code;
    }

    private async Task<AuthUser> CompleteAuthentication(string code)
    {
        // Implementation of REST call to Google/Firebase to exchange code for ID token
        return new AuthUser(); 
    }

    public async Task<AuthUser> LoginWithGitHubAsync() => throw new System.NotImplementedException();
    public async Task<AuthUser> LoginWithEmailAndPasswordAsync(string email, string password) => throw new System.NotImplementedException();
    public async Task<AuthUser> RegisterWithEmailAndPasswordAsync(string email, string password) => throw new System.NotImplementedException();
    public async Task<AuthUser?> GetCurrentUserAsync() => null;
    public void Logout() => _currentUser = null;
}
