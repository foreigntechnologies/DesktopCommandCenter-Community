using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IWhisperTranscriptionService
{
    /// <summary>
    /// Starts recording audio from the default microphone and transcribes it in real-time or when stopped.
    /// </summary>
    void StartRecording();

    /// <summary>
    /// Stops recording and returns the transcribed text.
    /// </summary>
    Task<string> StopRecordingAndTranscribeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Transcribes an existing audio file (WAV/MP3).
    /// </summary>
    Task<string> TranscribeFileAsync(string filePath, CancellationToken cancellationToken = default);
}
