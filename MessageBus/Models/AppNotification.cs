using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models
{
    public class AppNotification
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        //[BsonSerializer(typeof(BsonDocumentToObjectSerializer))]
        public object? ObjectId { get; set; }
        public string EventType { get; set; }//niepotrzebne
        public SimpleUser NotificationPartner { get; set; }
        public string NotificationType { get; set; }
        //public object? Data { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Displayed { get; set; } = false;
        public string Description { get; set; }
    }
    //public class BsonDocumentToObjectSerializer : SerializerBase<object>
    //{
    //    public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    //    {
    //        var bsonDocument = BsonDocumentSerializer.Instance.Deserialize(context);
    //        return BsonTypeMapper.MapToDotNetValue(bsonDocument);
    //    }

    //    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    //    {
    //        var bsonValue = BsonTypeMapper.MapToBsonValue(value);
    //        BsonDocumentSerializer.Instance.Serialize(context, bsonValue.AsBsonDocument);
    //    }
    //}
}
