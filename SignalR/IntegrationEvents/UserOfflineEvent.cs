using MessageBus;
using SignalR.Models;

namespace SignalR.IntegrationEvents;

public class UserOfflineEvent : IntegrationEvent
{
    public UserOfflineEvent(UserDto user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
    }

    public UserDto User { get; }
}
