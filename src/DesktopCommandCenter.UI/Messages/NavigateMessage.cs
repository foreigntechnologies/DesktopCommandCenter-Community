using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DesktopCommandCenter.UI.Messages;

public class NavigateMessage : ValueChangedMessage<string>
{
    public NavigateMessage(string actionId) : base(actionId)
    {
    }
}
