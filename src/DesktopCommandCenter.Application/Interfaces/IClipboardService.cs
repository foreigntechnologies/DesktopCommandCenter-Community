using System;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IClipboardService
{
    void StartMonitoring();
    void StopMonitoring();
    event EventHandler<string> TextCopied;
}
