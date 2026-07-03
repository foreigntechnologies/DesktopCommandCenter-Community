using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class AwakePage : Page
{
    public ViewModels.AwakeViewModel ViewModel { get; }

    public AwakePage()
    {
ViewModel = App.Current.Services.GetRequiredService<ViewModels.AwakeViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

        private void UpdateTranslations()
        {
            AwakePageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Awake_PageTitle");
            AwakeSubTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Awake_SubTitle");
            AwakeDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("Awake_Desc");
            AwakeToggleElement.Header = Helpers.LocalizationHelper.Instance.GetString("Awake_Toggle");
            var onText_AwakeToggleElement = Helpers.LocalizationHelper.Instance.GetString("Toggle_On");
            if (!string.IsNullOrEmpty(onText_AwakeToggleElement) && onText_AwakeToggleElement != "Toggle_On") AwakeToggleElement.OnContent = onText_AwakeToggleElement;
            var offText_AwakeToggleElement = Helpers.LocalizationHelper.Instance.GetString("Toggle_Off");
            if (!string.IsNullOrEmpty(offText_AwakeToggleElement) && offText_AwakeToggleElement != "Toggle_Off") AwakeToggleElement.OffContent = offText_AwakeToggleElement;
        }
}


