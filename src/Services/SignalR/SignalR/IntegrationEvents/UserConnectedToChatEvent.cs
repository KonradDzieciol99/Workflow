using MessageBus;
using SignalR.Commons.Models;

namespace SignalR.IntegrationEvents;

public record UserConnectedToChatEvent(UserDto ConnectedUser, string RecipientEmail) : IntegrationEvent;