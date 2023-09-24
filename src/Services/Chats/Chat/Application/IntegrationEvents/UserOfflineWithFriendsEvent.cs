using Chat.Application.Common.Models;
using MessageBus;

namespace Chat.Application.IntegrationEvents;

public record UserOfflineWithFriendsEvent(UserDto User, IEnumerable<UserDto> UserChatFriends)
    : IntegrationEvent;
