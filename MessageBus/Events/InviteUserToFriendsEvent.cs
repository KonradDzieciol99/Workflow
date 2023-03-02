using Mango.MessageBus;
using MediatR;
using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class InviteUserToFriendsEvent : BaseMessage,IRequest, IUserPersistentNotification
    {
        public FriendInvitationDtoGlobal FriendInvitationDto { get; set; }
        public SimpleUser UserWhoInvited { get; set; }
        public SimpleUser InvitedUser { get; set; }
        public bool? IsAccepted { get; set; }

    }
}
