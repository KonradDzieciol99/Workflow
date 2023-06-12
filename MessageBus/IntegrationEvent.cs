using MediatR;
using MessageBus.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus
{
    public class IntegrationEvent : IRequest
    {
        public IntegrationEvent()
        {
            
        }

        public IntegrationEvent(string? id)
        {
            Id = id;
        }

        public string? Id { get; set; }
        public DateTime MessageCreated { get; set; }
        //public SimpleUser? EventRecipient { get; set; }
        //public string? NotificationPartnerUserId { get; set; }
        //public string? NotificationPartnerUserEmail { get; set; }
        //public string? NotificationPartnerUserPhotoUrl { get; set; }

        //public SimpleUser? EventSender { get; set; } = new SimpleUser() { UserId = "System", UserEmail = "System" };
        public string? EventSenderUserId { get; set; }
        public string? EventSenderUserEmail { get; set; }
        public string? EventSenderUserPhotoUrl { get; set; }

        public string EventType { get; set; }
        public object? ObjectId { get; set; }//streamId

        //public SimpleUser? EventSender { get; set; }
        //w zależności od tego czy zakceptowane czy nie zakceptowane czy oczekujące
        //będzie sie zmieniał po prostu rozaj eventu 
    }
}
