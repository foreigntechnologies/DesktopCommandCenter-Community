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
}
