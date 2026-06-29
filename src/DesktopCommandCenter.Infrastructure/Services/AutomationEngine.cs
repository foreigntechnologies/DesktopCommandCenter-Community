using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Notifications;

namespace DesktopCommandCenter.Infrastructure.Services;

public class AutomationEngine : IAutomationEngine
{
    private readonly ILogger<AutomationEngine> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IClipboardService _clipboardService;
    private readonly string _filePath;
    private readonly object _lock = new();
    
    private List<AutomacaoRegra> _regras = new();
    private DeviceWatcher? _usbWatcher;
    private System.Threading.Timer? _processTimer;
    private readonly HashSet<string> _runningProcesses = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<System.Threading.Timer> _periodicTimers = new();
    private bool _isExecutingAction = false;
    private string? _lastClipboardText = null;

    public AutomationEngine(IServiceProvider serviceProvider, IClipboardService clipboardService, ILogger<AutomationEngine> logger)
    {
        _serviceProvider = serviceProvider;
        _clipboardService = clipboardService;
        _logger = logger;
        
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DCC");
        _filePath = Path.Combine(dir, "dcc_automations.json");
    }

    public void Start()
    {
        lock (_lock)
        {
            LoadRules();
            
            // 1. Escutar Área de Transferência
            _clipboardService.TextCopied += OnTextCopied;

            // 2. Iniciar USB Watcher
            StartUsbWatcher();

            // 3. Iniciar Process Poller (verifica a cada 4 segundos)
            _processTimer = new System.Threading.Timer(_ => PollProcesses(), null, 2000, 4000);

            // 4. Iniciar Timers Periódicos
            SetupPeriodicTimers();

            // 5. Gatilho: Ao ligar o computador (Executa imediatamente no startup)
            ExecuteStartupRules();
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            _clipboardService.TextCopied -= OnTextCopied;

            if (_usbWatcher != null)
            {
                _usbWatcher.Added -= OnUsbDeviceAdded;
                try { _usbWatcher.Stop(); } catch { }
                _usbWatcher = null;
            }

            _processTimer?.Dispose();
            _processTimer = null;

            ClearPeriodicTimers();
        }
    }

    public void ReloadRules()
    {
        lock (_lock)
        {
            Stop();
            Start();
        }
    }

    private void LoadRules()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<List<AutomacaoRegra>>(json);
                if (loaded != null)
                {
                    _regras = loaded;
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar regras de automação.");
        }

        // Regras default se não existir arquivo
        _regras = new List<AutomacaoRegra>
        {
            new AutomacaoRegra(Guid.NewGuid(), DateTime.UtcNow, null, "Ao copiar um link do YouTube", "Extrair ID do vídeo", "", "", true),
            new AutomacaoRegra(Guid.NewGuid(), DateTime.UtcNow, null, "Ao tirar um printscreen", "Extrair texto de imagem via OCR", "", "", false),
            new AutomacaoRegra(Guid.NewGuid(), DateTime.UtcNow, null, "Ao plugar Pendrive", "Exibir notificação do sistema (Toast)", "", "Novo dispositivo USB conectado!", true)
        };
        SaveRules();
    }

    private void SaveRules()
    {
        try
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var json = JsonSerializer.Serialize(_regras, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar regras de automação.");
        }
    }

    #region Event Listeners

    private void OnTextCopied(object? sender, string text)
    {
        if (_isExecutingAction) return;

        // Evitar execução duplicada para o mesmo texto
        if (text == _lastClipboardText) return;
        _lastClipboardText = text;

        if (text == "Imagem")
        {
            // Gatilho: Ao tirar um printscreen
            ExecuteRulesForTrigger("Ao tirar um printscreen", text);
        }
        else if (IsYouTubeUrl(text))
        {
            // Gatilho: Ao copiar um link do YouTube
            ExecuteRulesForTrigger("Ao copiar um link do YouTube", text);
        }
    }

    private void StartUsbWatcher()
    {
        try
        {
            // Interface class GUID para Discos/Armazenamento USB
            string aqsSelector = "System.Devices.InterfaceClassGuid:=\"{53f56307-b6bf-11d0-94f2-00a0c91efb8b}\"";
            _usbWatcher = DeviceInformation.CreateWatcher(aqsSelector);
            _usbWatcher.Added += OnUsbDeviceAdded;
            _usbWatcher.Start();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar USB watcher.");
        }
    }

    private async void OnUsbDeviceAdded(DeviceWatcher sender, DeviceInformation args)
    {
        // Aguarda a montagem da unidade
        await Task.Delay(2500);

        try
        {
            var removableDrives = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Removable && d.IsReady);

            foreach (var drive in removableDrives)
            {
                ExecuteRulesForTrigger("Ao plugar Pendrive", drive.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar inserção de USB.");
        }
    }

    private void PollProcesses()
    {
        try
        {
            var currentProcesses = System.Diagnostics.Process.GetProcesses()
                .Select(p => p.ProcessName)
                .Distinct()
                .ToList();

            foreach (var proc in currentProcesses)
            {
                if (!_runningProcesses.Contains(proc))
                {
                    _runningProcesses.Add(proc);
                    
                    // Gatilho: Ao abrir um aplicativo específico (compara com o nome do processo/executável)
                    ExecuteAppRules(proc);
                }
            }

            _runningProcesses.RemoveWhere(p => !currentProcesses.Contains(p));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar processos em execução.");
        }
    }

    private void SetupPeriodicTimers()
    {
        ClearPeriodicTimers();

        var periodicRules = _regras.Where(r => r.IsAtivo && r.Gatilho == "A cada X minutos/horas").ToList();
        foreach (var rule in periodicRules)
        {
            if (int.TryParse(rule.GatilhoParametro, out int minutes) && minutes > 0)
            {
                var ms = minutes * 60 * 1000;
                var timer = new System.Threading.Timer(_ => 
                {
                    ExecuteRuleAction(rule, "");
                }, null, ms, ms);
                
                _periodicTimers.Add(timer);
            }
        }
    }

    private void ClearPeriodicTimers()
    {
        foreach (var timer in _periodicTimers)
        {
            timer.Dispose();
        }
        _periodicTimers.Clear();
    }

    private void ExecuteStartupRules()
    {
        ExecuteRulesForTrigger("Ao ligar o computador", "");
    }

    #endregion

    #region Rule Execution Engine

    private void ExecuteRulesForTrigger(string trigger, string triggerValue)
    {
        var activeRules = _regras.Where(r => r.IsAtivo && r.Gatilho.Equals(trigger, StringComparison.OrdinalIgnoreCase));
        foreach (var rule in activeRules)
        {
            ExecuteRuleAction(rule, triggerValue);
        }
    }

    private void ExecuteAppRules(string processName)
    {
        var activeRules = _regras.Where(r => r.IsAtivo && r.Gatilho == "Ao abrir um aplicativo específico");
        foreach (var rule in activeRules)
        {
            var configuredParam = rule.GatilhoParametro.Trim();
            // Compara removendo o ".exe" se necessário
            var cleanParam = configuredParam.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? configuredParam[..^4]
                : configuredParam;

            if (cleanParam.Equals(processName, StringComparison.OrdinalIgnoreCase))
            {
                ExecuteRuleAction(rule, processName);
            }
        }
    }

    private async void ExecuteRuleAction(AutomacaoRegra rule, string triggerValue)
    {
        try
        {
            _logger.LogInformation($"Executando regra: Gatilho '{rule.Gatilho}' -> Ação '{rule.Acao}'");
            
            switch (rule.Acao)
            {
                case "Extrair ID do vídeo":
                    await ActionExtractYouTubeId(triggerValue);
                    break;

                case "Extrair texto de imagem via OCR":
                    await ActionExtractOcr();
                    break;

                case "Falar texto (Text-to-Speech)":
                    ActionSpeakText(rule.AcaoParametro);
                    break;

                case "Executar script PowerShell ou CMD":
                    ActionRunScript(rule.AcaoParametro);
                    break;

                case "Limpar Área de Transferência":
                    ActionClearClipboard();
                    break;

                case "Exibir notificação do sistema (Toast)":
                    ActionShowToast("Desktop Command Center", string.IsNullOrEmpty(rule.AcaoParametro) ? "Automação acionada!" : rule.AcaoParametro);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao rodar ação da regra {rule.Id}");
        }
    }

    #endregion

    #region Specific Actions Implementation

    private async Task ActionExtractYouTubeId(string url)
    {
        var id = ExtractYouTubeIdString(url);
        if (!string.IsNullOrEmpty(id))
        {
            _isExecutingAction = true;
            try
            {
                var package = new Windows.ApplicationModel.DataTransfer.DataPackage();
                package.SetText(id);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
                _lastClipboardText = id;
                
                ActionShowToast("YouTube Link Detectado", $"ID Extraído: {id} (Copiado)");
            }
            finally
            {
                _isExecutingAction = false;
            }
        }
    }

    private async Task ActionExtractOcr()
    {
        try
        {
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Bitmap))
            {
                var bitmapRef = await dataPackageView.GetBitmapAsync();
                using var stream = await bitmapRef.OpenReadAsync();
                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                // Cria pasta Prints em LocalApplicationData
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var printsFolder = await localFolder.CreateFolderAsync("Prints", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var fileName = $"print_{Guid.NewGuid()}.png";
                var file = await printsFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);

                using (var destStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId, destStream);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();
                }

                // Executa OCR do Windows
                var ocrEngine = Windows.Media.Ocr.OcrEngine.TryCreateFromUserProfileLanguages();
                if (ocrEngine != null)
                {
                    var ocrResult = await ocrEngine.RecognizeAsync(softwareBitmap);
                    if (!string.IsNullOrEmpty(ocrResult.Text))
                    {
                        _isExecutingAction = true;
                        try
                        {
                            var package = new Windows.ApplicationModel.DataTransfer.DataPackage();
                            package.SetText(ocrResult.Text);
                            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
                            _lastClipboardText = ocrResult.Text;
                            
                            ActionShowToast("OCR Concluído", $"Texto copiado ({ocrResult.Text.Length} caracteres)");
                        }
                        finally
                        {
                            _isExecutingAction = false;
                        }
                    }
                    else
                    {
                        ActionShowToast("OCR Concluído", "Nenhum texto reconhecido na imagem.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao rodar OCR na imagem do Clipboard.");
        }
    }

    private async void ActionSpeakText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        try
        {
            using var synthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            var stream = await synthesizer.SynthesizeTextToStreamAsync(text);
            
            var mediaPlayer = new Windows.Media.Playback.MediaPlayer();
            mediaPlayer.Source = Windows.Media.Core.MediaSource.CreateFromStream(stream, stream.ContentType);
            mediaPlayer.Play();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no TTS da automação.");
        }
    }

    private void ActionRunScript(string scriptPath)
    {
        if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath)) return;
        try
        {
            var isPowerShell = scriptPath.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase);
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = isPowerShell ? "powershell.exe" : "cmd.exe",
                Arguments = isPowerShell 
                    ? $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"" 
                    : $"/c \"{scriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            System.Diagnostics.Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao rodar script: {scriptPath}");
        }
    }

    private void ActionClearClipboard()
    {
        _isExecutingAction = true;
        try
        {
            Windows.ApplicationModel.DataTransfer.Clipboard.Clear();
            _lastClipboardText = null;
            ActionShowToast("Segurança", "Área de transferência limpa.");
        }
        finally
        {
            _isExecutingAction = false;
        }
    }

    private void ActionShowToast(string title, string content)
    {
        try
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var textNodes = toastXml.GetElementsByTagName("text");
            textNodes[0].AppendChild(toastXml.CreateTextNode(title));
            textNodes[1].AppendChild(toastXml.CreateTextNode(content));
            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exibir notificação Toast.");
        }
    }

    #endregion

    #region Helper Methods

    private bool IsYouTubeUrl(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        return text.Contains("youtube.com", StringComparison.OrdinalIgnoreCase) || 
               text.Contains("youtu.be", StringComparison.OrdinalIgnoreCase);
    }

    private string? ExtractYouTubeIdString(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var match = Regex.Match(url, 
            @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/ ]{11})", 
            RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }

    #endregion
}
