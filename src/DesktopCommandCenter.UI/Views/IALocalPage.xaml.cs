using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.UI.Views;

public sealed partial class IALocalPage : Page
{
    public ViewModels.IALocalViewModel ViewModel { get; }

    public IALocalPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ViewModels.IALocalViewModel>();
        this.InitializeComponent();
        Loaded += IALocalPage_Loaded;
        DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();
    }

    private void IALocalPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        UpdateTranslations();
    }

    private void UpdateTranslations()
    {
        var loc = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance;
        TooltipAttachImage.Content = loc.GetString("Chat_AttachImage");
        TooltipAttachAudio.Content = loc.GetString("Chat_TranscribeAudio");
        TooltipMic.Content = loc.GetString("Chat_SpeakMic");
        TxtPromptInput.PlaceholderText = loc.GetString("Chat_Placeholder");
        BtnSend.Content = loc.GetString("Chat_Send");
    }
}
