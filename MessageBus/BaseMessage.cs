using MediatR;
using MessageBus.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class BaseMessage: IRequest
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public DateTime MessageCreated { get; set; }
        public SimpleUser? NotificationRecipient { get; set; }
        public SimpleUser? NotificationSender { get; set; }
        public string EventType { get; set; }
        public string? ObjectId { get; set; }//streamId
        //w zależności od tego czy zakceptowane czy nie zakceptowane czy oczekujące
        //będzie sie zmieniał po prostu rozaj eventu 
    }
}
