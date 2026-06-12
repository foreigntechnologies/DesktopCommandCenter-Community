using DesktopCommandCenter.Application.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace DesktopCommandCenter.ProFeatures.Services;

public class FirestoreLicenseService : ILicenseService
{
    private readonly IAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly string _projectId = "desktop-command-center-windows"; // Extraído do JSON do Service Account

    public FirestoreLicenseService(IAuthService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient();
    }

    public async Task<bool> IsProUserAsync()
    {
        var plan = await GetCurrentPlanAsync();
        return plan.Equals("pro", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> GetCurrentPlanAsync()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null || string.IsNullOrEmpty(user.IdToken))
                return "free"; // Se não está logado, usa a versão local gratuita

            // Usa a API REST do Firestore passando o Token de Autenticação (OAuth Bearer)
            var firestoreUrl = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{user.Uid}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, firestoreUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.IdToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Se o documento não existe (404), o usuário acabou de se registrar e ainda não tem plano PRO
                return "free";
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonContent);

            // A estrutura REST do Firestore encapsula strings no objeto "stringValue"
            if (doc.RootElement.TryGetProperty("fields", out var fields))
            {
                if (fields.TryGetProperty("plan", out var planElement))
                {
                    if (planElement.TryGetProperty("stringValue", out var value))
                    {
                        return value.GetString() ?? "free";
                    }
                }
            }

            return "free";
        }
        catch (Exception)
        {
            // Em caso de erro de rede (offline), rebaixa para "free" temporariamente ou exibe aviso.
            // Para "Local First", podemos ter um cache local depois, mas por ora assumimos "free".
            return "free";
        }
    }
}
