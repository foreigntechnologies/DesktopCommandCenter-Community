using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class ColorPickerPage : Page
{
    public ViewModels.ColorPickerViewModel ViewModel { get; }

    public ColorPickerPage()
    {
InitializeComponent();
            UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.ColorPickerViewModel>();
        
    }

        private void UpdateTranslations()
        {
            ColorPickerPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("ColorPicker_PageTitle");
            ColorPickerToolTipCopyHexElement.Content = Helpers.LocalizationHelper.Instance.GetString("ColorPicker_ToolTipCopyHex");
            ColorPickerToolTipCopyRgbElement.Content = Helpers.LocalizationHelper.Instance.GetString("ColorPicker_ToolTipCopyRgb");
            if (ColorPickerBtnToggleElement.Content is string || ColorPickerBtnToggleElement.Content == null) ColorPickerBtnToggleElement.Content = Helpers.LocalizationHelper.Instance.GetString("ColorPicker_BtnToggle");
            ColorPickerDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("ColorPicker_Desc");
        }
}

