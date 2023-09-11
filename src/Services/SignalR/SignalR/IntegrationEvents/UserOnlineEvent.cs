using MessageBus;
using SignalR.Commons.Models;

namespace SignalR.IntegrationEvents;

public record UserOnlineEvent(UserDto OnlineUser) : IntegrationEvent;