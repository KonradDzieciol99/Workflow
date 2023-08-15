using MediatR;
using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NewOnlineMessagesUserEvent : IntegrationEvent, IRequest
    {
        public UserDto NewOnlineUser { get; set; }
        public IEnumerable<UserDto> NewOnlineUserChatFriends { get; set; }
    }
}
