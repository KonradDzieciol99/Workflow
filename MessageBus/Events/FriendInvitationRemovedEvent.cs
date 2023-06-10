using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class FriendInvitationRemovedEvent : IntegrationEvent
    {
        public FriendInvitationRemovedEvent(string actionInitiatorUserId, string actionInitiatorUserEmail, string? actionInitiatorUserPhotoUrl, string friendToRemoveUserId, string friendToRemoveUserEmail, string? friendToRemoveUserPhotoUrl)
        {
            ActionInitiatorUserId = actionInitiatorUserId ?? throw new ArgumentNullException(nameof(actionInitiatorUserId));
            ActionInitiatorUserEmail = actionInitiatorUserEmail ?? throw new ArgumentNullException(nameof(actionInitiatorUserEmail));
            ActionInitiatorUserPhotoUrl = actionInitiatorUserPhotoUrl;
            FriendToRemoveUserId = friendToRemoveUserId ?? throw new ArgumentNullException(nameof(friendToRemoveUserId));
            FriendToRemoveUserEmail = friendToRemoveUserEmail ?? throw new ArgumentNullException(nameof(friendToRemoveUserEmail));
            FriendToRemoveUserPhotoUrl = friendToRemoveUserPhotoUrl;
        }

        public string  ActionInitiatorUserId { get; set; }
        public string  ActionInitiatorUserEmail { get; set; }
        public string? ActionInitiatorUserPhotoUrl { get; set; }

        public string  FriendToRemoveUserId { get; set; }
        public string  FriendToRemoveUserEmail { get; set; }
        public string? FriendToRemoveUserPhotoUrl { get; set; }
    }
}
