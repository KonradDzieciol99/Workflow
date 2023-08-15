using MediatR;
using MessageBus.Models;

namespace MessageBus.Events;

public class UserOfflineEvent : IntegrationEvent
{
    public UserDto User { get; set; }
}
