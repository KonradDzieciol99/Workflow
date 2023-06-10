﻿using MessageBus.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Notification.Domain.Common.Exceptions;
using Notification.Domain.Common.Models;

namespace Notification.Domain.Entity;

public class AppNotification : BaseEntity
{
    private AppNotification() { }

    public AppNotification(string userId, /*object? objectId,*/ string notificationType, DateTime creationDate, string description, string? notificationPartnerId, string? notificationPartnerEmail, string? notificationPartnerPhotoUrl, bool displayed = false)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        //ObjectId = objectId;
        NotificationType = notificationType ?? throw new ArgumentNullException(nameof(notificationType));
        CreationDate = creationDate;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        NotificationPartnerId = notificationPartnerId;
        NotificationPartnerEmail = notificationPartnerEmail;
        NotificationPartnerPhotoUrl = notificationPartnerPhotoUrl;
        Displayed = displayed;
    }

    public string UserId { get; private set; }
    //public object? ObjectId { get; private set; }
    public string NotificationType { get; private set; }
    public DateTime CreationDate { get; private set; }
    public bool Displayed { get; private set; }
    public string Description { get; private set; }

    public string? NotificationPartnerId { get; set; }
    public string? NotificationPartnerEmail { get; set; }
    public string? NotificationPartnerPhotoUrl { get; set; }

    public void MarkAsSeen()
    {
        if (Displayed == true)
            throw new NotificationDomainException("Notification is already marked as read.");

        Displayed = true;
    }




}