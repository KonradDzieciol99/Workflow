using MessageBus;
using SignalR.Commons.Models;

namespace SignalR.IntegrationEvents;

public record UserOfflineEvent(UserDto User) : IntegrationEvent;