using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class HotkeyConfigItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _actionId = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _currentHotkeyDisplay = string.Empty;

    /// <summary>
    /// Se true, é uma funcionalidade PRO e deve exibir o cadeado.
    /// </summary>
    public bool IsProFeature { get; set; } = false;

    /// <summary>
    /// Controla se o botão de editar atalho está habilitado.
    /// Funcionalidades PRO ficam desabilitadas enquanto o plano for Free.
    /// </summary>
    [ObservableProperty]
    private bool _isEnabled = true;

    /// <summary>
    /// Visibilidade do ícone de cadeado (Visible se IsProFeature e não tem PRO).
    /// </summary>
    public Visibility LockVisibility => IsProFeature ? Visibility.Visible : Visibility.Collapsed;

    public uint Modifiers { get; set; }
    public uint VirtualKey { get; set; }
}
