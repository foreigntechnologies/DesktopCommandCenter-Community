using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class CapturaPage : Page
{
    public ViewModels.CapturaViewModel ViewModel { get; }

    public CapturaPage()
    {
ViewModel = App.Current.Services.GetRequiredService<ViewModels.CapturaViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

        private void UpdateTranslations()
        {
            CapturePageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Capture_PageTitle");
            CaptureSubTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Capture_SubTitle");
            CaptureDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("Capture_Desc");
            if (CaptureBtnStartElement.Content is string || CaptureBtnStartElement.Content == null) CaptureBtnStartElement.Content = Helpers.LocalizationHelper.Instance.GetString("Capture_BtnStart");
        }
}


