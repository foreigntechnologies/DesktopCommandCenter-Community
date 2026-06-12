using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;
using NAudio.Wave;
using Whisper.net;

namespace DesktopCommandCenter.ProFeatures.Services;

public class WhisperTranscriptionService : IWhisperTranscriptionService, IDisposable
{
    private WaveInEvent? _waveIn;
    private MemoryStream? _audioMemoryStream;
    private WaveFileWriter? _waveFileWriter;

    // TODO: This path should eventually be configurable or the model downloaded dynamically.
    private const string ModelPath = "ggml-base.bin";

    public void StartRecording()
    {
        if (_waveIn != null) return;

        _audioMemoryStream = new MemoryStream();
        // Whisper.net requere amostras em 16kHz, mono, 16-bit
        var waveFormat = new WaveFormat(16000, 16, 1);
        
        _waveIn = new WaveInEvent
        {
            WaveFormat = waveFormat
        };
        
        _waveFileWriter = new WaveFileWriter(_audioMemoryStream, waveFormat);

        _waveIn.DataAvailable += (s, a) =>
        {
            _waveFileWriter.Write(a.Buffer, 0, a.BytesRecorded);
        };

        _waveIn.StartRecording();
    }

    public async Task<string> StopRecordingAndTranscribeAsync(CancellationToken cancellationToken = default)
    {
        if (_waveIn == null || _waveFileWriter == null || _audioMemoryStream == null)
            return string.Empty;

        _waveIn.StopRecording();
        _waveIn.Dispose();
        _waveIn = null;

        await _waveFileWriter.FlushAsync(cancellationToken);
        _waveFileWriter.Dispose();
        _waveFileWriter = null;

        // Reset memory stream to beginning
        _audioMemoryStream.Position = 0;

        // Transcribe
        return await TranscribeStreamAsync(_audioMemoryStream, cancellationToken);
    }

    public async Task<string> TranscribeFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        // For simplicity, we assume the file is a compatible WAV file (16kHz, 16-bit, mono) 
        // OR the WhisperProcessor handles it if we decode it. 
        // Normally for MP3/WAV, we'd use MediaFoundationReader to decode to 16kHz WAV first.
        
        using var reader = new MediaFoundationReader(filePath);
        var outFormat = new WaveFormat(16000, 16, 1);
        using var resampler = new MediaFoundationResampler(reader, outFormat);
        using var tempStream = new MemoryStream();
        
        WaveFileWriter.WriteWavFileToStream(tempStream, resampler);
        tempStream.Position = 0;

        return await TranscribeStreamAsync(tempStream, cancellationToken);
    }

    private async Task<string> TranscribeStreamAsync(Stream wavStream, CancellationToken cancellationToken)
    {
        if (!File.Exists(ModelPath))
        {
            return $"[Erro: Modelo Whisper não encontrado em {ModelPath}. É necessário baixar o ggml-base.bin e colocar na pasta principal do aplicativo.]";
        }

        using var whisperFactory = WhisperFactory.FromPath(ModelPath);
        using var processor = whisperFactory.CreateBuilder()
            .WithLanguage("pt") // Definindo o idioma padrão para português
            .Build();

        var sb = new StringBuilder();
        
        await foreach(var result in processor.ProcessAsync(wavStream, cancellationToken))
        {
            sb.Append(result.Text);
            sb.Append(' ');
        }

        return sb.ToString().Trim();
    }

    public void Dispose()
    {
        _waveIn?.Dispose();
        _waveFileWriter?.Dispose();
        _audioMemoryStream?.Dispose();
    }
}
