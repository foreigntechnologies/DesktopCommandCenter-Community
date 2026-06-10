using Microsoft.UI.Xaml.Controls;
using DesktopCommandCenter.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<SettingsViewModel>();
        this.InitializeComponent();
    }

    private void HotkeyTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        e.Handled = true;

        if (sender is TextBox textBox && textBox.Tag is HotkeyConfigItemViewModel item)
        {
            var virtualKey = e.Key;
            
            // Ignore modifiers pressed alone
            if (virtualKey == Windows.System.VirtualKey.Control || 
                virtualKey == Windows.System.VirtualKey.Menu || 
                virtualKey == Windows.System.VirtualKey.Shift || 
                virtualKey == Windows.System.VirtualKey.LeftWindows || 
                virtualKey == Windows.System.VirtualKey.RightWindows)
            {
                return;
            }

            // Capture current modifiers
            var ctrl = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            var alt = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Menu).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            var shift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            var win = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.LeftWindows).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) ||
                      Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.RightWindows).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

            uint modifiers = 0;
            if (alt) modifiers |= 1;
            if (ctrl) modifiers |= 2;
            if (shift) modifiers |= 4;
            if (win) modifiers |= 8;

            // Convert to uint
            uint vk = (uint)virtualKey;

            // If Backspace or Delete is pressed without modifiers, clear the hotkey
            if (!ctrl && !alt && !shift && !win && (virtualKey == Windows.System.VirtualKey.Back || virtualKey == Windows.System.VirtualKey.Delete))
            {
                modifiers = 0;
                vk = 0;
            }

            ViewModel.SaveHotkey(item, modifiers, vk);
        }
    }
}
