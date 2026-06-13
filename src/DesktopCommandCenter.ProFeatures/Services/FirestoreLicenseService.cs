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
            {
                Serilog.Log.Debug("GetCurrentPlanAsync: No user logged in or empty IdToken.");
                return "free";
            }

            // Usa a API REST do Firestore passando o Token de Autenticação (OAuth Bearer)
            var firestoreUrl = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{user.Uid}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, firestoreUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.IdToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Serilog.Log.Warning("Firestore license check failed. Status: {StatusCode} ({ReasonPhrase}). Body: {Body}", 
                    response.StatusCode, response.ReasonPhrase, errorBody);
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
                        var plan = value.GetString() ?? "free";
                        Serilog.Log.Information("Firestore license check succeeded. User plan: {Plan}", plan);
                        return plan;
                    }
                }
            }

            Serilog.Log.Warning("Firestore license check succeeded but 'plan' field not found in response fields. JSON: {JsonContent}", jsonContent);
            return "free";
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Erro ao obter o plano atual no FirestoreLicenseService.");
            return "free";
        }
    }
}
