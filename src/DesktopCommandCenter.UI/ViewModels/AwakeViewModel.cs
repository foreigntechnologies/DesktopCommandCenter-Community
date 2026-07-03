using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.InteropServices;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class AwakeViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isAwakeEnabled;

    [ObservableProperty]
    private string _statusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Awake_StatusDisabled");

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
            StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Awake_StatusEnabled");
        }
        else
        {
            // Restaura o comportamento padrão
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            StatusMessage = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Awake_StatusDisabled");
        }
    }
}
