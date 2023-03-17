using MessageBus.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Notification.Models
{
    public class AppNotificationMongo
    {
        [BsonId]
        public string Id { get; set; }
        public string UserId { get; set; }
        public BsonDocument? ObjectId { get; set; }
        public string EventType { get; set; }//niepotrzebne
        public SimpleUser NotificationPartner { get; set; }
        public string NotificationType { get; set; }
        //public string? Data { get; set; }//niepotrzebne ??
        public DateTime CreationDate { get; set; }
        public bool Displayed { get; set; } = false;
        public string Description { get; set; }
    }

}
