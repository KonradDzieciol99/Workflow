﻿using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NewOfflineUserWithFriendsEvent
    {
        public SimpleUser User { get; set; }
        public IEnumerable<SimpleUser> UserChatFriends { get; set; }
    }
}
