using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class ChatMessage : ObservableObject
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsUser => Role == "User";
}

public partial class IALocalViewModel : ObservableObject
{
    [ObservableProperty]
    private string _currentPrompt = "";

    [ObservableProperty]
    private bool _isGenerating = false;

    [ObservableProperty]
    private string _statusMessage = "Pronto para usar o motor Ollama (localhost:11434).";

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentPrompt)) return;

        var userMsg = new ChatMessage { Role = "User", Content = CurrentPrompt };
        Messages.Add(userMsg);
        
        var promptBackup = CurrentPrompt;
        CurrentPrompt = "";
        IsGenerating = true;
        StatusMessage = "Gerando resposta...";

        try
        {
            using var client = new HttpClient();
            var requestBody = new
            {
                model = "llama3", // Assuming user might have llama3, you can change it
                prompt = promptBackup,
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            // Tentativa de chamada real para Ollama local. Se não existir, vai falhar e cair no catch mockado.
            var response = await client.PostAsync("http://localhost:11434/api/generate", content);
            response.EnsureSuccessStatusCode();
            
            var jsonStr = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonStr);
            var responseText = doc.RootElement.GetProperty("response").GetString() ?? "";

            Messages.Add(new ChatMessage { Role = "AI", Content = responseText });
            StatusMessage = "Pronto.";
        }
        catch
        {
            // Fallback mockado para caso Ollama não esteja rodando (para o usuário ver funcionando na V1)
            await Task.Delay(1500);
            Messages.Add(new ChatMessage { Role = "AI", Content = $"[Ollama não detectado no localhost]\n\nSimulação de resposta para o seu prompt:\n'{promptBackup}'\n\nInstale o Ollama para a experiência real!" });
            StatusMessage = "Offline (Simulação)";
        }
        finally
        {
            IsGenerating = false;
        }
    }
}
