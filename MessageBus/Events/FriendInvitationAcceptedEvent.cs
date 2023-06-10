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
    public class FriendInvitationAcceptedEvent : IntegrationEvent
    {
        public FriendInvitationAcceptedEvent(string invitationSendingUserId, string invitationSendingUserEmail, string? invitationSendingUserPhotoUrl, string invitationAcceptingUserId, string invitationAcceptingUserEmail, string? invitationAcceptingUserPhotoUrl)
        {
            InvitationSendingUserId = invitationSendingUserId ?? throw new ArgumentNullException(nameof(invitationSendingUserId));
            InvitationSendingUserEmail = invitationSendingUserEmail ?? throw new ArgumentNullException(nameof(invitationSendingUserEmail));
            InvitationSendingUserPhotoUrl = invitationSendingUserPhotoUrl;
            InvitationAcceptingUserId = invitationAcceptingUserId ?? throw new ArgumentNullException(nameof(invitationAcceptingUserId));
            InvitationAcceptingUserEmail = invitationAcceptingUserEmail ?? throw new ArgumentNullException(nameof(invitationAcceptingUserEmail));
            InvitationAcceptingUserPhotoUrl = invitationAcceptingUserPhotoUrl;
        }

        public string InvitationSendingUserId { get; set; }
        public string InvitationSendingUserEmail { get; set; }
        public string? InvitationSendingUserPhotoUrl { get; set; }

        public string InvitationAcceptingUserId { get; set; }
        public string InvitationAcceptingUserEmail { get; set; }
        public string? InvitationAcceptingUserPhotoUrl { get; set; }
    }
}
