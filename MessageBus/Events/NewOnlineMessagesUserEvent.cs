using MediatR;
using MessageBus.Models;
using System.Collections.Generic;

namespace MessageBus.Events
{
    public class NewOnlineMessagesUserEvent : IntegrationEvent, IRequest
    {
        public UserDto NewOnlineUser { get; set; }
        public IEnumerable<UserDto> NewOnlineUserChatFriends { get; set; }
    }
}
