using CommunityToolkit.Mvvm.ComponentModel;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class HotkeyConfigItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _actionId = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _currentHotkeyDisplay = string.Empty;

    public uint Modifiers { get; set; }
    public uint VirtualKey { get; set; }
}
