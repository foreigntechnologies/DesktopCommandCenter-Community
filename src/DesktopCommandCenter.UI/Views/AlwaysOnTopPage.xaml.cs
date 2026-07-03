using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AlwaysOnTopPage : Page
{
    public ViewModels.AlwaysOnTopViewModel ViewModel { get; }

    public AlwaysOnTopPage()
    {
ViewModel = App.Current.Services.GetRequiredService<ViewModels.AlwaysOnTopViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

        private void UpdateTranslations()
        {
            AlwaysOnTopPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("AlwaysOnTop_PageTitle");
            AlwaysOnTopSubTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("AlwaysOnTop_SubTitle");
            AlwaysOnTopDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("AlwaysOnTop_Desc");
            AlwaysOnTopToggleElement.Header = Helpers.LocalizationHelper.Instance.GetString("AlwaysOnTop_Toggle");
            var onText_AlwaysOnTopToggleElement = Helpers.LocalizationHelper.Instance.GetString("Toggle_On");
            if (!string.IsNullOrEmpty(onText_AlwaysOnTopToggleElement) && onText_AlwaysOnTopToggleElement != "Toggle_On") AlwaysOnTopToggleElement.OnContent = onText_AlwaysOnTopToggleElement;
            var offText_AlwaysOnTopToggleElement = Helpers.LocalizationHelper.Instance.GetString("Toggle_Off");
            if (!string.IsNullOrEmpty(offText_AlwaysOnTopToggleElement) && offText_AlwaysOnTopToggleElement != "Toggle_Off") AlwaysOnTopToggleElement.OffContent = offText_AlwaysOnTopToggleElement;
        }
}


