﻿using MediatR;
using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NewOnlineMessagesUserEvent : BaseMessage, IRequest
    {
        public SimpleUser NewOnlineUser { get; set; }
        public IEnumerable<SimpleUser> NewOnlineUserChatFriends { get; set; }
    }
}
