using MediatR;
using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NewOnlineUserWithFriendsEvent : IRequest
    {
        public SimpleUser NewOnlineUser { get; set; }
        public IEnumerable<SimpleUser> NewOnlineUserChatFriends { get; set; }
    }
}
