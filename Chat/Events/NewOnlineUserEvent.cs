using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NewOnlineUserEvent
    {
        public IEnumerable<FriendInvitationDto> FriendInvitationDtos { get; set; }
    }
}
