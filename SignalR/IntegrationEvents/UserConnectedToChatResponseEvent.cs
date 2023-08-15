using MessageBus;
using MessageBus.Models;
using SignalR.Models;

namespace SignalR.IntegrationEvents;

public class UserConnectedToChatResponseEvent : IntegrationEvent
{
    public UserConnectedToChatResponseEvent(UserDto connectedUser, string recipientEmail, List<MessageDto> messages)
    {
        ConnectedUser = connectedUser ?? throw new ArgumentNullException(nameof(connectedUser));
        RecipientEmail = recipientEmail ?? throw new ArgumentNullException(nameof(recipientEmail));
        Messages = messages ?? throw new ArgumentNullException(nameof(messages));
    }

    public UserDto ConnectedUser { get; set; }
    public string RecipientEmail { get; set; }
    public List<MessageDto> Messages { get; set; }
}
