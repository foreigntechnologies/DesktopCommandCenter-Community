using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public class ChatMessageDto
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public interface IIAAgentService
{
    /// <summary>
    /// Sends a prompt to the agent and streams the response back.
    /// </summary>
    IAsyncEnumerable<string> SendMessageStreamAsync(string prompt, string? imagePath = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an audio file path to be transcribed and processed by the agent.
    /// </summary>
    IAsyncEnumerable<string> SendAudioStreamAsync(string audioFilePath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Clears the chat history context.
    /// </summary>
    void ClearHistory();

    /// <summary>
    /// Gets a list of available AI models installed locally (e.g., from Ollama).
    /// </summary>
    Task<List<string>> GetAvailableModelsAsync();

    /// <summary>
    /// Sets the active AI model to be used by the agent.
    /// </summary>
    void SetModel(string modelId);

    /// <summary>
    /// Pulls (downloads) an AI model from the registry (e.g., Ollama).
    /// </summary>
    Task PullModelAsync(string modelName, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}
