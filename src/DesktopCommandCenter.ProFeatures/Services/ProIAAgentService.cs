using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace DesktopCommandCenter.ProFeatures.Services;

public class ProIAAgentService : IIAAgentService
{
    private ChatHistory _chatHistory;

    public ProIAAgentService()
    {
        _chatHistory = new ChatHistory("Você é a IA assistente integrada ao Desktop Command Center. Você é útil, direto e fala português do Brasil.");
    }

    private string GetSetting(string fileName, string defaultValue)
    {
        try
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
            var filePath = Path.Combine(dir, fileName);
            if (File.Exists(filePath))
            {
                var val = File.ReadAllText(filePath).Trim();
                if (!string.IsNullOrEmpty(val)) return val;
            }
        }
        catch { }
        return defaultValue;
    }

    public async IAsyncEnumerable<string> SendMessageStreamAsync(string prompt, string? imagePath = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _chatHistory.AddUserMessage(prompt);

        string provider = GetSetting("dcc_ai_provider.txt", "Ollama");
        string apiKey = GetSetting("dcc_ai_apikey.txt", "");
        string model = GetSetting("dcc_ai_model.txt", "");

        // Se o usuário não tem chave de API e selecionou outro provedor, fallback para Ollama
        if (string.IsNullOrEmpty(apiKey) && provider != "Ollama")
        {
            provider = "Ollama";
        }

        string fullResponse = "";

        if (provider == "Claude")
        {
            // Implementação nativa via HttpClient para Anthropic Claude
            await foreach (var chunk in SendClaudeStreamAsync(prompt, string.IsNullOrEmpty(model) ? "claude-3-5-sonnet-20240620" : model, apiKey, cancellationToken))
            {
                fullResponse += chunk;
                yield return chunk;
            }
        }
        else
        {
            // Usar Semantic Kernel para OpenAI, Gemini e Ollama
            var builder = Kernel.CreateBuilder();

            if (provider == "OpenAI")
            {
                builder.AddOpenAIChatCompletion(
                    modelId: string.IsNullOrEmpty(model) ? "gpt-4o" : model,
                    apiKey: apiKey
                );
            }
            else if (provider == "Gemini")
            {
#pragma warning disable SKEXP0070
                builder.AddGoogleAIGeminiChatCompletion(
                    modelId: string.IsNullOrEmpty(model) ? "gemini-1.5-pro" : model,
                    apiKey: apiKey
                );
#pragma warning restore SKEXP0070
            }
            else // Ollama
            {
                builder.AddOpenAIChatCompletion(
                    modelId: string.IsNullOrEmpty(model) ? "llama3" : model,
                    apiKey: "bypass",
                    endpoint: new Uri("http://localhost:11434/v1")
                );
            }

            var kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            await foreach (var content in chatCompletionService.GetStreamingChatMessageContentsAsync(_chatHistory, kernel: kernel, cancellationToken: cancellationToken))
            {
                if (content.Content != null)
                {
                    fullResponse += content.Content;
                    yield return content.Content;
                }
            }
        }

        _chatHistory.AddAssistantMessage(fullResponse);
    }

    private async IAsyncEnumerable<string> SendClaudeStreamAsync(string prompt, string model, string apiKey, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        
        // Converter ChatHistory para o formato do Claude
        var messages = new List<object>();
        foreach (var msg in _chatHistory)
        {
            if (msg.Role == AuthorRole.User || msg.Role == AuthorRole.Assistant)
            {
                messages.Add(new { role = msg.Role.Label.ToLower(), content = msg.Content });
            }
        }

        var requestBody = new
        {
            model = model,
            max_tokens = 4096,
            system = "Você é a IA assistente integrada ao Desktop Command Center. Você é útil, direto e fala português do Brasil.",
            messages = messages,
            stream = true
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages")
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            yield return $"[Erro do Claude API: {response.StatusCode}] {error}";
            yield break;
        }

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                if (data == "[DONE]") break;
                
                string? chunk = null;
                try
                {
                    using var doc = JsonDocument.Parse(data);
                    if (doc.RootElement.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "content_block_delta")
                    {
                        if (doc.RootElement.TryGetProperty("delta", out var delta) && delta.TryGetProperty("text", out var text))
                        {
                            chunk = text.GetString() ?? "";
                        }
                    }
                }
                catch { }

                if (chunk != null)
                {
                    yield return chunk;
                }
            }
        }
    }

    public async IAsyncEnumerable<string> SendAudioStreamAsync(string audioFilePath, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return "O recurso de voz está sendo implementado...";
        await Task.CompletedTask;
    }

    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatHistory.AddSystemMessage("Você é a IA assistente integrada ao Desktop Command Center. Você é útil, direto e fala português do Brasil.");
    }
}
