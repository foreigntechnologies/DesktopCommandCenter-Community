using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class TradutorPage : Page
{
    public ViewModels.TradutorViewModel ViewModel { get; }

    public TradutorPage()
    {
ViewModel = App.Current.Services.GetRequiredService<ViewModels.TradutorViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

        private void UpdateTranslations()
        {
            TranslatorPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Translator_PageTitle");
            TranslatorSourceTextElement.Header = Helpers.LocalizationHelper.Instance.GetString("Translator_SourceText");
            var p_TranslatorSourceTextElement = Helpers.LocalizationHelper.Instance.GetString("Translator_SourceText_Placeholder");
            if (!string.IsNullOrEmpty(p_TranslatorSourceTextElement) && p_TranslatorSourceTextElement != "Translator_SourceText_Placeholder") TranslatorSourceTextElement.PlaceholderText = p_TranslatorSourceTextElement;
            TranslatorBtnPasteElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnPaste");
            TranslatorBtnPasteTargetElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnPaste");
            TranslatorBtnCopyElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnCopy");
            TranslatorBtnCopyTargetElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnCopy");
            TranslatorTargetTextElement.Header = Helpers.LocalizationHelper.Instance.GetString("Translator_TargetText");
            var p_TranslatorTargetTextElement = Helpers.LocalizationHelper.Instance.GetString("Translator_TargetText_Placeholder");
            if (!string.IsNullOrEmpty(p_TranslatorTargetTextElement) && p_TranslatorTargetTextElement != "Translator_TargetText_Placeholder") TranslatorTargetTextElement.PlaceholderText = p_TranslatorTargetTextElement;
            TranslatorBtnPasteElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnPaste");
            TranslatorBtnPasteTargetElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnPaste");
            TranslatorBtnCopyElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnCopy");
            TranslatorBtnCopyTargetElement.Content = Helpers.LocalizationHelper.Instance.GetString("Translator_BtnCopy");
        }
}



