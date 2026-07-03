using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class TemporizadorPage : Page
{
    public ViewModels.TemporizadorViewModel ViewModel { get; }

    public TemporizadorPage()
    {
ViewModel = App.Current.Services.GetRequiredService<ViewModels.TemporizadorViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

        private void UpdateTranslations()
        {
            TemporizadorPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Temporizador_PageTitle");
            // TODO: Implement translation for TemporizadorInputYearsElement of type NumberBox
            // TODO: Implement translation for TemporizadorInputMonthsElement of type NumberBox
            // TODO: Implement translation for TemporizadorInputDaysElement of type NumberBox
            // TODO: Implement translation for TemporizadorInputHoursElement of type NumberBox
            // TODO: Implement translation for TemporizadorInputMinutesElement of type NumberBox
            // TODO: Implement translation for TemporizadorInputSecondsElement of type NumberBox
            // TODO: Implement translation for TemporizadorInputMillisecondsElement of type NumberBox
            if (TemporizadorBtnStartElement.Content is string || TemporizadorBtnStartElement.Content == null) TemporizadorBtnStartElement.Content = Helpers.LocalizationHelper.Instance.GetString("Temporizador_BtnStart");
            if (TemporizadorBtnPauseElement.Content is string || TemporizadorBtnPauseElement.Content == null) TemporizadorBtnPauseElement.Content = Helpers.LocalizationHelper.Instance.GetString("Temporizador_BtnPause");
            if (TemporizadorBtnResetElement.Content is string || TemporizadorBtnResetElement.Content == null) TemporizadorBtnResetElement.Content = Helpers.LocalizationHelper.Instance.GetString("Temporizador_BtnReset");
        }
}


