﻿using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NotificationAddedEvent : IntegrationEvent
    {
        public NotificationAddedEvent(string id, string userId, /*object? objectId,*/ string notificationType, DateTime creationDate, bool displayed, string description, string? notificationPartnerUserId, string? notificationPartnerUserEmail, string? notificationPartnerUserPhotoUrl)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            //ObjectId = objectId;
            NotificationType = notificationType ?? throw new ArgumentNullException(nameof(notificationType));
            CreationDate = creationDate;
            Displayed = displayed;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            NotificationPartnerUserId = notificationPartnerUserId;
            NotificationPartnerUserEmail = notificationPartnerUserEmail;
            NotificationPartnerUserPhotoUrl = notificationPartnerUserPhotoUrl;
        }

        public string Id { get; private set; }
        public string UserId { get; private set; }
        //public object? ObjectId { get; private set; }

        public string NotificationType { get; private set; }
        public DateTime CreationDate { get; private set; }
        public bool Displayed { get; private set; } 
        public string Description { get; private set; }

        public string? NotificationPartnerUserId { get; set; }
        public string? NotificationPartnerUserEmail { get; set; }
        public string? NotificationPartnerUserPhotoUrl { get; set; }
    }
}