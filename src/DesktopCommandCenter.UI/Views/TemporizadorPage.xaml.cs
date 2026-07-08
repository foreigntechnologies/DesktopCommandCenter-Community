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
            TemporizadorInputYearsElement.Header = Helpers.LocalizationHelper.Instance.GetString("Temporizador_InputYears");
            TemporizadorInputMonthsElement.Header = Helpers.LocalizationHelper.Instance.GetString("Temporizador_InputMonths");
            TemporizadorInputDaysElement.Header = Helpers.LocalizationHelper.Instance.GetString("Temporizador_InputDays");
            TemporizadorInputHoursElement.Header = Helpers.LocalizationHelper.Instance.GetString("Temporizador_InputHours");
            TemporizadorInputMinutesElement.Header = Helpers.LocalizationHelper.Instance.GetString("Temporizador_InputMinutes");
            TemporizadorInputSecondsElement.Header = Helpers.LocalizationHelper.Instance.GetString("Temporizador_InputSeconds");
            TemporizadorInputMillisecondsElement.Header = Helpers.LocalizationHelper.Instance.GetString("Temporizador_InputMilliseconds");
            if (TemporizadorBtnStartElement.Content is string || TemporizadorBtnStartElement.Content == null) TemporizadorBtnStartElement.Content = Helpers.LocalizationHelper.Instance.GetString("Temporizador_BtnStart");
            if (TemporizadorBtnPauseElement.Content is string || TemporizadorBtnPauseElement.Content == null) TemporizadorBtnPauseElement.Content = Helpers.LocalizationHelper.Instance.GetString("Temporizador_BtnPause");
            if (TemporizadorBtnResetElement.Content is string || TemporizadorBtnResetElement.Content == null) TemporizadorBtnResetElement.Content = Helpers.LocalizationHelper.Instance.GetString("Temporizador_BtnReset");
        }
}


