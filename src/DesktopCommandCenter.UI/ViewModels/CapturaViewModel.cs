using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class CapturaViewModel : ObservableObject
{
    [RelayCommand]
    private void CaptureScreen()
    {
        // Usa o protocolo do Windows Snipping Tool para abrir direto no modo de captura
        Process.Start(new ProcessStartInfo("ms-screenclip:") { UseShellExecute = true });
    }
}
