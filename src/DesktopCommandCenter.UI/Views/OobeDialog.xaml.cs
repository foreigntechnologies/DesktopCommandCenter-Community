using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class OobeDialog : ContentDialog
{
    public OobeDialog()
    {
        this.InitializeComponent();
        
        this.PrimaryButtonClick += OobeDialog_PrimaryButtonClick;
    }

    private void OobeDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        string themeStr = ThemeComboBox.SelectedIndex switch
        {
            0 => "Light",
            1 => "Dark",
            _ => "Default"
        };
        
        App.SaveTheme(themeStr);
        App.ApplyTheme(themeStr);
    }
}
