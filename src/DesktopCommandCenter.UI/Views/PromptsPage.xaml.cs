using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class PromptsPage : Page
{
    public ViewModels.PromptsViewModel ViewModel { get; }

    public PromptsPage()
    {
ViewModel = App.Current.Services.GetRequiredService<ViewModels.PromptsViewModel>();
        this.InitializeComponent();
        UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
        this.Loaded += async (s, e) => await ViewModel.LoadPromptsAsync();
    }

        private void UpdateTranslations()
        {
            PromptsPageTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Prompts_PageTitle");
// Removed
            
            
            PromptsEmptyTitleElement.Text = Helpers.LocalizationHelper.Instance.GetString("Prompts_EmptyTitle");
            PromptsEmptyDescElement.Text = Helpers.LocalizationHelper.Instance.GetString("Prompts_EmptyDesc");
            PromptsInputTitleElement.Header = Helpers.LocalizationHelper.Instance.GetString("Prompts_InputTitle");
            var p_PromptsInputTitleElement = Helpers.LocalizationHelper.Instance.GetString("Prompts_InputTitle_Placeholder");
            if (!string.IsNullOrEmpty(p_PromptsInputTitleElement) && p_PromptsInputTitleElement != "Prompts_InputTitle_Placeholder") PromptsInputTitleElement.PlaceholderText = p_PromptsInputTitleElement;
            PromptsInputCatElement.Header = Helpers.LocalizationHelper.Instance.GetString("Prompts_InputCat");
            var p_PromptsInputCatElement = Helpers.LocalizationHelper.Instance.GetString("Prompts_InputCat_Placeholder");
            if (!string.IsNullOrEmpty(p_PromptsInputCatElement) && p_PromptsInputCatElement != "Prompts_InputCat_Placeholder") PromptsInputCatElement.PlaceholderText = p_PromptsInputCatElement;
            PromptsInputContentElement.Header = Helpers.LocalizationHelper.Instance.GetString("Prompts_InputContent");
            var p_PromptsInputContentElement = Helpers.LocalizationHelper.Instance.GetString("Prompts_InputContent_Placeholder");
            if (!string.IsNullOrEmpty(p_PromptsInputContentElement) && p_PromptsInputContentElement != "Prompts_InputContent_Placeholder") PromptsInputContentElement.PlaceholderText = p_PromptsInputContentElement;
            if (PromptsBtnSaveElement.Content is string || PromptsBtnSaveElement.Content == null) PromptsBtnSaveElement.Content = Helpers.LocalizationHelper.Instance.GetString("Prompts_BtnSave");
            if (PromptsBtnCancelElement.Content is string || PromptsBtnCancelElement.Content == null) PromptsBtnCancelElement.Content = Helpers.LocalizationHelper.Instance.GetString("Prompts_BtnCancel");
        }
}




