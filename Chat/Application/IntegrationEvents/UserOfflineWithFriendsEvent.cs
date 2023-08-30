using Chat.Application.Common.Models;
using MessageBus;

namespace Chat.Application.IntegrationEvents;

public class UserOfflineWithFriendsEvent : IntegrationEvent
{
    public UserOfflineWithFriendsEvent(UserDto user, IEnumerable<UserDto> userChatFriends)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        UserChatFriends = userChatFriends ?? throw new ArgumentNullException(nameof(userChatFriends));
    }

    public UserDto User { get; set; }
    public IEnumerable<UserDto> UserChatFriends { get; set; }
}
