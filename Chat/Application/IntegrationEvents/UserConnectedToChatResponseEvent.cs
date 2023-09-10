using Chat.Application.Common.Models;
using MessageBus;

namespace Chat.Application.IntegrationEvents;

public record UserConnectedToChatResponseEvent(UserDto ConnectedUser, string RecipientEmail, List<MessageDto> Messages) : IntegrationEvent;