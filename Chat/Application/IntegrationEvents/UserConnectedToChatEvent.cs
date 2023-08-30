using Chat.Application.Common.Models;
using MessageBus;

namespace Chat.Application.IntegrationEvents;

public class UserConnectedToChatEvent : IntegrationEvent
{
    public UserConnectedToChatEvent(UserDto connectedUser, string recipientEmail)
    {
        ConnectedUser = connectedUser ?? throw new ArgumentNullException(nameof(connectedUser));
        RecipientEmail = recipientEmail ?? throw new ArgumentNullException(nameof(recipientEmail));
    }

    public UserDto ConnectedUser { get; set; }
    public string RecipientEmail { get; set; }
}
