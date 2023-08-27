using MessageBus;
using SignalR.Models;

namespace SignalR.IntegrationEvents;

public class UserOfflineEvent : IntegrationEvent
{
    public UserDto User { get; set; }
}
