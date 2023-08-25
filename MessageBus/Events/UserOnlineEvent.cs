using MessageBus.Models;
using System;

namespace MessageBus.Events;

public class UserOnlineEvent : IntegrationEvent
{
    public UserOnlineEvent(UserDto onlineUser)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
    }

    public UserDto OnlineUser { get; set; }
}
