using System;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IHotkeyService
{
    /// <summary>
    /// Registra um atalho global que disparará o Action fornecido.
    /// Modifiers: 1=Alt, 2=Ctrl, 4=Shift, 8=Win
    /// Key: Virtual Key Code (Ex: 'C' = 0x43)
    /// </summary>
    bool RegisterHotkey(int modifier, int key, Action callback);

    /// <summary>
    /// Limpa todos os atalhos registrados atualmente.
    /// </summary>
    void UnregisterAll();
}
