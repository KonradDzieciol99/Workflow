using MessageBus;
using MessageBus.Models;

namespace SignalR.IntegrationEvents;

public class NewOnlineMessagesUserWithFriendsEvent : IntegrationEvent
{
    public IEnumerable<UserDto> NewOnlineUserChatFriends { get; set; }
    public UserDto NewOnlineUser { get; set; }
}
