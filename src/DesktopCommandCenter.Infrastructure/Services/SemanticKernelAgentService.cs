using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Infrastructure.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace DesktopCommandCenter.Infrastructure.Services;

public class SemanticKernelAgentService : IIAAgentService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ChatHistory _chatHistory;

    public SemanticKernelAgentService()
    {
        var builder = Kernel.CreateBuilder();

        // Configuração usando endpoint compatível OpenAI nativo do Ollama (/v1).
        // Isso permite uso maduro de ToolCallBehavior.
        builder.AddOpenAIChatCompletion(
            modelId: "llama3",
            apiKey: "not-needed", // Ollama ignora apiKey local
            endpoint: new Uri("http://localhost:11434/v1")
        );

        // Registra o nosso plugin local nativo (Ferramentas do Sistema)
        builder.Plugins.AddFromType<SystemInfoPlugin>("SystemInfo");

        _kernel = builder.Build();
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        _chatHistory = new ChatHistory("Você é um assistente prestativo do Desktop Command Center. Você pode acessar os arquivos e ver as horas usando suas ferramentas.");
    }

    public async IAsyncEnumerable<string> SendMessageStreamAsync(string prompt, string? imagePath = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            // Nota: Para enviar imagens via Ollama, o modelo configurado deve ser multi-modal, como "llava" ou "llama3.2-vision".
            // Se o usuário enviar imagem com modelo llama3 normal, pode ocorrer um erro do endpoint do Ollama.
            var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath, cancellationToken);
            var imageMimeType = "image/" + System.IO.Path.GetExtension(imagePath)?.TrimStart('.') ?? "png";
            
            var userMessage = new ChatMessageContentItemCollection
            {
                new TextContent(prompt),
                new ImageContent(imageBytes, imageMimeType)
            };
            
            _chatHistory.AddUserMessage(userMessage);
        }
        else
        {
            _chatHistory.AddUserMessage(prompt);
        }

        // Habilita a invocação automática das ferramentas (ex: SystemInfoPlugin)
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var responseStream = _chatCompletionService.GetStreamingChatMessageContentsAsync(
            _chatHistory, 
            executionSettings, 
            _kernel, 
            cancellationToken);

        string fullResponse = string.Empty;

        await foreach (var content in responseStream)
        {
            if (content.Content != null)
            {
                fullResponse += content.Content;
                yield return content.Content;
            }
        }

        if (!string.IsNullOrEmpty(fullResponse))
        {
            _chatHistory.AddAssistantMessage(fullResponse);
        }
    }

    public async IAsyncEnumerable<string> SendAudioStreamAsync(string audioFilePath, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Neste design, a UI usa o WhisperTranscriptionService para transcrever o áudio localmente,
        // e em seguida passa o texto gerado para SendMessageStreamAsync().
        // Logo, a lógica aqui pode ser meramente um repasse se injetarmos o WhisperService ou
        // deixamos a UI orquestrar. Vamos usar a UI para orquestrar e chamar SendMessageStreamAsync diretamente.
        yield break; 
    }

    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatHistory.AddSystemMessage("Você é um assistente prestativo do Desktop Command Center. Você pode acessar os arquivos e ver as horas usando suas ferramentas.");
    }
}
