using MessageBus.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Notification.Models
{
    public class AppNotificationMongo
    {
        [BsonId]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string? ObjectId { get; set; }
        public string EventType { get; set; }//niepotrzebne
        public SimpleUser NotificationPartner { get; set; }
        public string NotificationType { get; set; }
        public string? Data { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Displayed { get; set; } = false;
        public string Description { get; set; }
    }
}
