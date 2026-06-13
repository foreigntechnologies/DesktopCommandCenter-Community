using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace DesktopCommandCenter.ProFeatures.Services;

public class ProIAAgentService : IIAAgentService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ChatHistory _chatHistory;

    public ProIAAgentService()
    {
        // Configura o Semantic Kernel para usar a API compatível com OpenAI do Ollama (localhost)
        var builder = Kernel.CreateBuilder();
        
        builder.AddOpenAIChatCompletion(
            modelId: "llama3", // Modelo padrão do Ollama
            apiKey: "bypass",  // Ollama não exige API Key
            endpoint: new Uri("http://localhost:11434/v1")
        );

        _kernel = builder.Build();
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        
        _chatHistory = new ChatHistory("Você é a IA assistente integrada ao Desktop Command Center. Você é útil, direto e fala português do Brasil.");
    }

    public async IAsyncEnumerable<string> SendMessageStreamAsync(string prompt, string? imagePath = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _chatHistory.AddUserMessage(prompt);

        string fullResponse = "";
        
        await foreach (var content in _chatCompletionService.GetStreamingChatMessageContentsAsync(_chatHistory, kernel: _kernel, cancellationToken: cancellationToken))
        {
            if (content.Content != null)
            {
                fullResponse += content.Content;
                yield return content.Content;
            }
        }
        
        _chatHistory.AddAssistantMessage(fullResponse);
    }

    public async IAsyncEnumerable<string> SendAudioStreamAsync(string audioFilePath, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // To be implemented with Whisper.net
        yield return "O recurso de voz está sendo implementado...";
        await Task.CompletedTask;
    }

    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatHistory.AddSystemMessage("Você é a IA assistente integrada ao Desktop Command Center. Você é útil, direto e fala português do Brasil.");
    }
}
