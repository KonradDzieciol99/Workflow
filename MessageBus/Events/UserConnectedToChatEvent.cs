using MessageBus.Models;
using System;

namespace MessageBus.Events;

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
