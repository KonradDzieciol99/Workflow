using Chat.Application.Common.Models;
using MessageBus;

namespace Chat.Application.IntegrationEvents;

public class UserOnlineEvent : IntegrationEvent
{
    public UserOnlineEvent(UserDto onlineUser)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
    }

    public UserDto OnlineUser { get; set; }
}
