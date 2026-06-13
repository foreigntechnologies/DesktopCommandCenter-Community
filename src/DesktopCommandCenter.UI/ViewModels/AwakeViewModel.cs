using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.InteropServices;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class AwakeViewModel : ObservableObject
{
    [ObservableProperty] public partial bool IsAwakeEnabled { get; set; }

    [ObservableProperty]
    private string _statusMessage = "O PC pode dormir normalmente baseado nas configuraÃ§Ãµes do Windows.";

    [Flags]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    partial void OnIsAwakeEnabledChanged(bool value)
    {
        if (value)
        {
            // Impede a tela de desligar e o sistema de dormir
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            StatusMessage = "Awake estÃ¡ ATIVO. Sua tela e o PC nÃ£o vÃ£o desligar nem dormir.";
        }
        else
        {
            // Restaura o comportamento padrÃ£o
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            StatusMessage = "O PC pode dormir normalmente baseado nas configuraÃ§Ãµes do Windows.";
        }
    }
}

