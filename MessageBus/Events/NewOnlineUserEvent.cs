using MediatR;
using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Events
{
    public class NewOnlineUserEvent : BaseMessage, IRequest
    {
        public SimpleUser NewOnlineUser { get; set; }
        //public IEnumerable<User> NewOnlineUserChatFriends { get; set; }
    }
}
