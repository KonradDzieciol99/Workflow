using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models
{
    public class AppNotification
    {
        [BsonId]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ObjectId { get; set; }
        public string EventType { get; set; }//niepotrzebne
        public SimpleUser NotificationPartner { get; set; }
        public string NotificationType { get; set; }
        public object Data { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Displayed { get; set; } = false;
    }
}
