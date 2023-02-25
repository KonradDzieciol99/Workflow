using MediatR;
using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Events
{
    public class NewOnlineUserEvent : IRequest
    {
        public SimpleUser NewOnlineUser { get; set; }
        //public IEnumerable<User> NewOnlineUserChatFriends { get; set; }
    }
}
