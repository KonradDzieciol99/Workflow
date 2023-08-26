using MessageBus;
using MessageBus.Models;

namespace Notification.Application.IntegrationEvents;


public class UserOnlineEvent : IntegrationEvent
{
    public UserOnlineEvent(UserDto onlineUser)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
    }

    public UserDto OnlineUser { get; set; }
}

