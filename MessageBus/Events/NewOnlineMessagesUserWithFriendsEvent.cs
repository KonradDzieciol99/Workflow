using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Events
{
    public class NewOnlineMessagesUserWithFriendsEvent
    {
        public NewOnlineMessagesUserWithFriendsEvent()
        {
        }

        public IEnumerable<SimpleUser> NewOnlineUserChatFriends { get; set; }
        public SimpleUser NewOnlineUser { get; set; }
    }
}