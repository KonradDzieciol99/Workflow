using MongoDB.Bson.Serialization.Attributes;

namespace Notification.Models
{
    public class FriendInvitationId
    {
        [BsonElement("InviterUserId")]
        public string InviterUserId { get; set; }
        [BsonElement("InvitedUserId")]
        public string InvitedUserId { get; set; }
    }
}
