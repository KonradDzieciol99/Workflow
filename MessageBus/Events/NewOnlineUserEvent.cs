using MessageBus.Models;
using System.Collections.Generic;

namespace Chat.Events
{
    public class NewOnlineUserEvent
    {
        public User NewOnlineUser { get; set; }
        public IEnumerable<User> NewOnlineUserChatFriends { get; set; }
    }
}
