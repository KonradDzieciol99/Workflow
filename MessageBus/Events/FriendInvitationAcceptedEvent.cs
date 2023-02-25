using MediatR;
using MessageBus.Models;
using Microsoft.Azure.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class FriendInvitationAcceptedEvent : IRequest
    {
        public FriendInvitationDtoGlobal FriendInvitationDto { get; set; }
        public SimpleUser UserWhoseInvitationAccepted { get; set; }
        public SimpleUser UserWhoAcceptedInvitation { get; set; }
    }
}
