using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Events
{
    public class NewOnlineUserEvent
    {
        public SimpleUser NewOnlineUser { get; set; }
        //public IEnumerable<User> NewOnlineUserChatFriends { get; set; }
    }
}
