using MessageBus;
using MessageBus.Models;

namespace Chat.Application.IntegrationEvents;

public class UserOfflineEvent : IntegrationEvent
{
    public UserDto User { get; set; }
}