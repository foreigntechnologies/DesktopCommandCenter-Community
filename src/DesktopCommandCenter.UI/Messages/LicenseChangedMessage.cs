using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DesktopCommandCenter.UI.Messages;

public class LicenseChangedMessage : ValueChangedMessage<bool>
{
    public LicenseChangedMessage(bool isPro) : base(isPro)
    {
    }
}
