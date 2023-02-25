using MediatR;
using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Events
{
    public class NewOnlineMessagesUserWithFriendsEvent : IRequest
    {
        public NewOnlineMessagesUserWithFriendsEvent()
        {
        }

        public IEnumerable<SimpleUser> NewOnlineUserChatFriends { get; set; }
        public SimpleUser NewOnlineUser { get; set; }
    }
}