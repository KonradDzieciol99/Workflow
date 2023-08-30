using Chat.Application.Common.Models;
using MessageBus;

namespace Chat.Application.IntegrationEvents;

public class UserOfflineEvent : IntegrationEvent
{
    public UserDto User { get; set; }
}