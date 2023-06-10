﻿using System;

namespace MessageBus.Events
{
    public class FriendInvitationAddedEvent : IntegrationEvent//, IUserPersistentNotification
    {
        public FriendInvitationAddedEvent(string invitationSendingUserId, string invitationSendingUserEmail, string? invitationSendingUserPhotoUrl, string invitedUserId, string invitedUserEmail, string? invitedUserPhotoUrl)
        {
            InvitationSendingUserId = invitationSendingUserId ?? throw new ArgumentNullException(nameof(invitationSendingUserId));
            InvitationSendingUserEmail = invitationSendingUserEmail ?? throw new ArgumentNullException(nameof(invitationSendingUserEmail));
            InvitationSendingUserPhotoUrl = invitationSendingUserPhotoUrl;
            InvitedUserId = invitedUserId ?? throw new ArgumentNullException(nameof(invitedUserId));
            InvitedUserEmail = invitedUserEmail ?? throw new ArgumentNullException(nameof(invitedUserEmail));
            InvitedUserPhotoUrl = invitedUserPhotoUrl;
        }

        //public FriendInvitationDtoGlobal FriendInvitationDto { get; set; }
        public string InvitationSendingUserId { get; set; }
        public string InvitationSendingUserEmail { get; set; }
        public string? InvitationSendingUserPhotoUrl { get; set; }

        public string InvitedUserId { get; set; }
        public string InvitedUserEmail { get; set; }
        public string? InvitedUserPhotoUrl { get; set; }

    }
}
