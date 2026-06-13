using DesktopCommandCenter.Application.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DesktopCommandCenter.ProFeatures.Services;

/// <summary>
/// Implementação do Firebase Auth via REST API pura (Firebase Identity Toolkit v1).
/// Usa OAuth 2.0 loopback (HttpListener em porta local) para login social com
/// Google e GitHub sem depender de sessionStorage do browser — compatível com
/// apps desktop nativos WinUI 3.
/// </summary>
public class FirebaseAuthService : IAuthService
{
    private readonly HttpClient _httpClient = new();

    // Firebase project credentials
    private const string ApiKey         = "AIzaSyDyAVbKJ8umZ_ezLHl9dCUWfE3cGIa-6zA";
    private const string IdentityBase   = "https://identitytoolkit.googleapis.com/v1/accounts";
    private const string TokenEndpoint  = "https://securetoken.googleapis.com/v1/token";

    // Sessão persistida localmente
    private AuthUser? _currentUser;
    private string?   _refreshToken;

    private static readonly string SessionFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DCC", "dcc_session.json");

    public bool IsAuthenticated  => _currentUser != null;
    public string? CurrentUserUid => _currentUser?.Uid;

    public FirebaseAuthService()
    {
        // Tenta restaurar a sessão salva em disco (login persistente entre reinicializações)
        Task.Run(TryRestoreSessionAsync);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SOCIAL LOGIN — OAuth 2.0 Loopback (Desktop-Native Pattern)
    // ─────────────────────────────────────────────────────────────────────────

    public Task<AuthUser> LoginWithGoogleAsync()  => LoginWithProviderAsync("google.com");
    public Task<AuthUser> LoginWithGitHubAsync()  => LoginWithProviderAsync("github.com");

    public Task<AuthUser> LinkWithGoogleAsync()   => LinkWithProviderAsync("google.com");
    public Task<AuthUser> LinkWithGitHubAsync()   => LinkWithProviderAsync("github.com");

    private async Task<AuthUser> LinkWithProviderAsync(string providerId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) throw new Exception("Nenhum usuário logado para vincular.");
        return await LoginWithProviderAsync(providerId, currentUser.IdToken);
    }

    /// <summary>
    /// Fluxo OAuth 2.0 para apps desktop:
    /// 1. Firebase createAuthUri       → URL de login do provedor (Google/GitHub)
    /// 2. Abre browser do sistema      → usuário autoriza
    /// 3. HttpListener local           → captura o callback na porta aleatória
    /// 4. Firebase signInWithIdp       → troca o callback por ID token
    /// Se um idToken for fornecido, a credencial será VINCULADA a este idToken (conta existente).
    /// </summary>
    private async Task<AuthUser> LoginWithProviderAsync(string providerId, string? idToken = null)
    {
        int port        = GetAvailablePort();
        string redirect = $"http://127.0.0.1:{port}/";
        string sessionId = Guid.NewGuid().ToString("N");

        // 1. Obter URL de autenticação do provedor via Firebase
        var authUriPayload = JsonSerializer.Serialize(new
        {
            providerId,
            continueUri  = redirect,
            oauthScope   = "email profile",
            sessionId    = sessionId
        });

        var createUriBody = await PostRawAsync($"{IdentityBase}:createAuthUri", authUriPayload);
        using var createUriDoc = JsonDocument.Parse(createUriBody);

        if (!createUriDoc.RootElement.TryGetProperty("authUri", out var authUriProp))
            throw new Exception("Firebase não retornou URL de autenticação. Verifique se o provedor está habilitado no Console Firebase.");

        var authUri = authUriProp.GetString()!;

        // 2. Começa a ouvir na porta local ANTES de abrir o browser
        var listenTask = ListenForCallbackAsync(port);

        // 3. Abre o browser do sistema
        Process.Start(new ProcessStartInfo(authUri) { UseShellExecute = true });

        // 4. Aguarda o callback (timeout: 5 minutos)
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        string callbackUrl;
        try
        {
            callbackUrl = await listenTask.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new Exception("Login cancelado: o tempo limite de 5 minutos foi atingido.");
        }

        // 5. Troca o callback URL por tokens Firebase (e vincula se idToken não for nulo)
        var payloadDict = new System.Collections.Generic.Dictionary<string, object>
        {
            ["requestUri"] = callbackUrl,
            ["sessionId"]  = sessionId,
            ["returnSecureToken"] = true,
            ["returnIdpCredential"] = true
        };
        if (!string.IsNullOrEmpty(idToken))
            payloadDict["idToken"] = idToken;

        var signInPayload = JsonSerializer.Serialize(payloadDict);

        var signInBody = await PostRawAsync($"{IdentityBase}:signInWithIdp", signInPayload);
        return await ParseAuthResponseAsync(signInBody);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // EMAIL / SENHA
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<AuthUser> LoginWithEmailAndPasswordAsync(string email, string password)
    {
        var json = JsonSerializer.Serialize(new { email, password, returnSecureToken = true });
        var body = await PostRawAsync($"{IdentityBase}:signInWithPassword", json);
        return await ParseAuthResponseAsync(body);
    }

    public async Task<AuthUser> RegisterWithEmailAndPasswordAsync(string email, string password)
    {
        var json = JsonSerializer.Serialize(new { email, password, returnSecureToken = true });
        var body = await PostRawAsync($"{IdentityBase}:signUp", json);
        return await ParseAuthResponseAsync(body);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SESSÃO
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<AuthUser?> GetCurrentUserAsync()
    {
        if (_currentUser == null) return null;

        // Tenta renovar o token se tivermos um refresh token
        if (_refreshToken != null)
        {
            try { await RefreshIdTokenAsync(); }
            catch { /* mantém o token atual; vai expirar naturalmente */ }
        }
        return _currentUser;
    }

    public void Logout()
    {
        _currentUser  = null;
        _refreshToken = null;
        try { if (File.Exists(SessionFile)) File.Delete(SessionFile); } catch { }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS INTERNOS
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<AuthUser> ParseAuthResponseAsync(string json)
    {
        using var doc  = JsonDocument.Parse(json);
        var root       = doc.RootElement;

        // Verifica erros da API Firebase
        if (root.TryGetProperty("error", out var err))
        {
            var msg = err.TryGetProperty("message", out var m) ? m.GetString() ?? "UNKNOWN" : "UNKNOWN";
            throw new Exception(TranslateError(msg));
        }

        var uid          = root.GetProperty("localId").GetString()!;
        var email        = root.TryGetProperty("email", out var ep) ? ep.GetString() ?? "" : "";
        var idToken      = root.GetProperty("idToken").GetString()!;
        var refreshToken = root.TryGetProperty("refreshToken", out var rp) ? rp.GetString() : null;

        _currentUser  = new AuthUser { Uid = uid, Email = email, IdToken = idToken };
        _refreshToken = refreshToken;

        await FetchProviderDataAsync(_currentUser);
        await PersistSessionAsync();
        return _currentUser;
    }

    private async Task FetchProviderDataAsync(AuthUser user)
    {
        if (string.IsNullOrEmpty(user.IdToken)) return;
        var json = JsonSerializer.Serialize(new { idToken = user.IdToken });
        try
        {
            var body = await PostRawAsync($"{IdentityBase}:lookup", json);
            using var doc = JsonDocument.Parse(body);
            var users = doc.RootElement.GetProperty("users");
            if (users.GetArrayLength() > 0)
            {
                var targetUser = users[0];
                if (targetUser.TryGetProperty("providerUserInfo", out var pInfos))
                {
                    user.Providers.Clear();
                    user.LinkedEmails.Clear();
                    foreach (var pInfo in pInfos.EnumerateArray())
                    {
                        var pid = pInfo.GetProperty("providerId").GetString();
                        var pEmail = pInfo.TryGetProperty("email", out var pEp) ? pEp.GetString() : null;
                        if (!string.IsNullOrEmpty(pid))
                        {
                            user.Providers.Add(pid);
                            if (!string.IsNullOrEmpty(pEmail))
                                user.LinkedEmails[pid] = pEmail;
                        }
                    }
                }
            }
        }
        catch { /* falha silenciosa */ }
    }

    private async Task RefreshIdTokenAsync()
    {
        var json = JsonSerializer.Serialize(new { grant_type = "refresh_token", refresh_token = _refreshToken });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{TokenEndpoint}?key={ApiKey}", content);
        var body = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.TryGetProperty("error", out _))
        {
            Logout();
            throw new Exception("Sessão expirada. Faça login novamente.");
        }

        var newIdToken      = doc.RootElement.GetProperty("id_token").GetString()!;
        var newRefreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!;

        _refreshToken = newRefreshToken;
        if (_currentUser != null)
        {
            _currentUser.IdToken = newIdToken;
            await FetchProviderDataAsync(_currentUser);
            await PersistSessionAsync();
        }
    }

    private async Task PersistSessionAsync()
    {
        if (_currentUser == null) return;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SessionFile)!);
            var data = JsonSerializer.Serialize(new
            {
                uid          = _currentUser.Uid,
                email        = _currentUser.Email,
                idToken      = _currentUser.IdToken,
                refreshToken = _refreshToken,
                providers    = _currentUser.Providers,
                linkedEmails = _currentUser.LinkedEmails
            });
            await File.WriteAllTextAsync(SessionFile, data);
        }
        catch { /* falha silenciosa */ }
    }

    private async Task TryRestoreSessionAsync()
    {
        try
        {
            if (!File.Exists(SessionFile)) return;
            var json = await File.ReadAllTextAsync(SessionFile);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var uid   = root.GetProperty("uid").GetString()!;
            var email = root.GetProperty("email").GetString()!;
            var rt    = root.TryGetProperty("refreshToken", out var rp) ? rp.GetString() : null;

            if (string.IsNullOrEmpty(rt)) return;

            _currentUser  = new AuthUser { Uid = uid, Email = email, IdToken = "" };
            _refreshToken = rt;

            if (root.TryGetProperty("providers", out var provs))
            {
                foreach (var p in provs.EnumerateArray())
                    _currentUser.Providers.Add(p.GetString()!);
            }
            if (root.TryGetProperty("linkedEmails", out var le))
            {
                foreach (var prop in le.EnumerateObject())
                    _currentUser.LinkedEmails[prop.Name] = prop.Value.GetString()!;
            }

            // Renova imediatamente
            await RefreshIdTokenAsync();
        }
        catch
        {
            _currentUser  = null;
            _refreshToken = null;
        }
    }

    private async Task<string> PostRawAsync(string endpoint, string jsonBody)
    {
        var content  = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{endpoint}?key={ApiKey}", content);
        return await response.Content.ReadAsStringAsync();
    }

    private static int GetAvailablePort()
    {
        return 5000;
    }

    private static async Task<string> ListenForCallbackAsync(int port)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Start();

        var context    = await listener.GetContextAsync();
        var fullUrl    = context.Request.Url?.ToString() ?? "";

        // Retorna página de feedback amigável ao browser
        const string html =
            "<html><head><meta charset='utf-8'><title>DCC</title><style>" +
            "body{font-family:system-ui,sans-serif;display:flex;flex-direction:column;" +
            "align-items:center;justify-content:center;height:100vh;margin:0;background:#0a0a0a;color:#fff}" +
            "h2{font-size:1.5rem;margin-bottom:.5rem}p{color:#aaa;font-size:.9rem}" +
            "</style></head><body>" +
            "<h2>✅ Login realizado com sucesso!</h2>" +
            "<p>Pode fechar esta aba e voltar ao Desktop Command Center.</p>" +
            "<script>setTimeout(()=>window.close(),2000);</script></body></html>";

        var bytes = Encoding.UTF8.GetBytes(html);
        context.Response.ContentType     = "text/html; charset=utf-8";
        context.Response.ContentLength64 = bytes.Length;
        await context.Response.OutputStream.WriteAsync(bytes);
        context.Response.Close();
        listener.Stop();

        return fullUrl;
    }

    private static string TranslateError(string code) => code switch
    {
        "EMAIL_NOT_FOUND"               => "E-mail não encontrado. Verifique ou crie uma conta.",
        "INVALID_PASSWORD"              => "Senha incorreta. Tente novamente.",
        "INVALID_EMAIL"                 => "E-mail inválido.",
        "USER_DISABLED"                 => "Esta conta foi desativada.",
        "EMAIL_EXISTS"                  => "Este e-mail já está em uso.",
        "TOO_MANY_ATTEMPTS_TRY_LATER"   => "Muitas tentativas. Aguarde e tente novamente.",
        var s when s.StartsWith("WEAK_PASSWORD") => "Senha fraca. Use no mínimo 6 caracteres.",
        _                               => code
    };
}
