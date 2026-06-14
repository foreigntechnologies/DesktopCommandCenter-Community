using System;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IAutomationEngine
{
    void Start();
    void Stop();
    void ReloadRules();
}
