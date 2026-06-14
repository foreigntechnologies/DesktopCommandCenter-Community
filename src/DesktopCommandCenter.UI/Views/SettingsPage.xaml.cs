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
        
        this.Loaded += SettingsPage_Loaded;
    }

    private void SettingsPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (App.IsProUnlocked)
        {
            ProBadge.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            CommunityBadge.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
        else
        {
            ProBadge.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            CommunityBadge.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        
        UpdateTranslations();
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += (s, args) => UpdateTranslations();
    }

    private void UpdateTranslations()
    {
        var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;

        TxtSettingsTitle.Text = loc.GetString("Settings_Title");
        TxtAppearanceTitle.Text = loc.GetString("Settings_AppearanceTitle");
        CmbTheme.Header = loc.GetString("Settings_Theme");
        CmbThemeItem1.Content = loc.GetString("Settings_ThemeLight");
        CmbThemeItem2.Content = loc.GetString("Settings_ThemeDark");
        CmbThemeItem3.Content = loc.GetString("Settings_ThemeSystem");
        CmbTimeFormat.Header = loc.GetString("Settings_TimeFormat");
        CmbTime1.Content = loc.GetString("Settings_Time1");
        CmbTime2.Content = loc.GetString("Settings_Time2");
        CmbTime3.Content = loc.GetString("Settings_Time3");
        CmbTime4.Content = loc.GetString("Settings_Time4");
        CmbDateFormat.Header = loc.GetString("Settings_DateFormat");
        CmbDate1.Content = loc.GetString("Settings_Date1");
        CmbDate2.Content = loc.GetString("Settings_Date2");
        CmbDate3.Content = loc.GetString("Settings_Date3");
        CmbDate4.Content = loc.GetString("Settings_Date4");
        TxtThemeDesc.Text = loc.GetString("Settings_ThemeDesc");
        TxtHotkeysTitle.Text = loc.GetString("Settings_HotkeysTitle");
        TxtHotkeysDesc.Text = loc.GetString("Settings_HotkeysDesc");
        TxtSubscriptionTitle.Text = loc.GetString("Settings_SubTitle");
        TxtCommunityTitle.Text = loc.GetString("Settings_CommTitle");
        TxtCommunitySubtitle.Text = loc.GetString("Settings_CommSub");
        TxtCommunityDesc.Text = loc.GetString("Settings_CommDesc");
        TxtCommunityBtn.Text = loc.GetString("Settings_CommBtn");
        TxtProBadgeTitle.Text = loc.GetString("Settings_ProBadge");
        TxtProRecommended.Text = loc.GetString("Settings_ProRec");
        TxtProSubtitle.Text = loc.GetString("Settings_ProSub");
        TxtProDesc.Text = loc.GetString("Settings_ProDesc");
        TxtProBtnNotLogged.Text = loc.GetString("Settings_ProNotLogged");
        TxtProBtnFree.Text = loc.GetString("Settings_ProFree");
        TxtProBtnActive.Text = loc.GetString("Settings_ProActive");
        TxtProBtnManage.Text = loc.GetString("Settings_ProManage");
        BtnLogoutSettings.Content = loc.GetString("Auth_Logout");
    }


    private HotkeyConfigItemViewModel? _editingItem;
    private uint _tempModifiers = 0;
    private uint _tempVirtualKey = 0;

    private async void EditHotkeyButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is HotkeyConfigItemViewModel item)
        {
            _editingItem = item;
            _tempModifiers = item.Modifiers;
            _tempVirtualKey = item.VirtualKey;

            HotkeyDialogActionText.Text = $"Configure as teclas para: {item.DisplayName}";
            HotkeyPreviewText.Text = string.IsNullOrEmpty(item.CurrentHotkeyDisplay) || item.CurrentHotkeyDisplay == "None" 
                ? "Nenhum" 
                : item.CurrentHotkeyDisplay;

            EditHotkeyDialog.XamlRoot = this.XamlRoot;
            await EditHotkeyDialog.ShowAsync();
        }
    }

    private void EditHotkeyDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (_editingItem != null)
        {
            ViewModel.SaveHotkey(_editingItem, _tempModifiers, _tempVirtualKey);
        }
    }

    private void ClearHotkeyButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _tempModifiers = 0;
        _tempVirtualKey = 0;
        HotkeyPreviewText.Text = "Nenhum";
    }

    private void EditHotkeyDialog_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        e.Handled = true;
        var virtualKey = e.Key;

        // Capture current modifiers state
        var ctrl = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
        var alt = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Menu).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
        var shift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
        var win = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.LeftWindows).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) ||
                  Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.RightWindows).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        // Standard actions for Enter and Escape when no modifiers are active
        if (!ctrl && !alt && !shift && !win)
        {
            if (virtualKey == Windows.System.VirtualKey.Enter)
            {
                EditHotkeyDialog.Hide();
                if (_editingItem != null)
                {
                    ViewModel.SaveHotkey(_editingItem, _tempModifiers, _tempVirtualKey);
                }
                return;
            }
            if (virtualKey == Windows.System.VirtualKey.Escape)
            {
                EditHotkeyDialog.Hide();
                return;
            }
        }

        // Ignore modifiers pressed alone, but update preview
        if (virtualKey == Windows.System.VirtualKey.Control || 
            virtualKey == Windows.System.VirtualKey.Menu || 
            virtualKey == Windows.System.VirtualKey.Shift || 
            virtualKey == Windows.System.VirtualKey.LeftWindows || 
            virtualKey == Windows.System.VirtualKey.RightWindows)
        {
            UpdateTempModifiersDisplay();
            return;
        }

        uint modifiers = 0;
        if (alt) modifiers |= 1;
        if (ctrl) modifiers |= 2;
        if (shift) modifiers |= 4;
        if (win) modifiers |= 8;

        _tempModifiers = modifiers;
        _tempVirtualKey = (uint)virtualKey;

        HotkeyPreviewText.Text = DesktopCommandCenter.Infrastructure.Services.GlobalHotkeyService.GetHotkeyString(modifiers, _tempVirtualKey);
    }

    private void UpdateTempModifiersDisplay()
    {
        var ctrl = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
        var alt = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Menu).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
        var shift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
        var win = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.LeftWindows).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) ||
                  Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.RightWindows).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        var parts = new System.Collections.Generic.List<string>();
        if (ctrl) parts.Add("Ctrl");
        if (alt) parts.Add("Alt");
        if (shift) parts.Add("Shift");
        if (win) parts.Add("Win");

        if (parts.Count > 0)
        {
            HotkeyPreviewText.Text = string.Join(" + ", parts) + " + ...";
        }
        else
        {
            HotkeyPreviewText.Text = "Nenhum";
        }
    }
    private void OpenCommunityRepo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
            "https://github.com/foreigntechnologies/DesktopCommandCenter-Community")
        { UseShellExecute = true });
    }
}
