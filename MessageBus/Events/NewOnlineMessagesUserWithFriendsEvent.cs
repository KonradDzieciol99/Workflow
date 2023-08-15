using MediatR;
using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Events
{
    public class NewOnlineMessagesUserWithFriendsEvent : IntegrationEvent, IRequest
    {
        public NewOnlineMessagesUserWithFriendsEvent()
        {
        }

        public IEnumerable<UserDto> NewOnlineUserChatFriends { get; set; }
        public UserDto NewOnlineUser { get; set; }
    }
}