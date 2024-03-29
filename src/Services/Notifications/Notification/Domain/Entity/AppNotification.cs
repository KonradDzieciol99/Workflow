﻿using Notification.Domain.Common.Enums;
using Notification.Domain.Common.Exceptions;
using Notification.Domain.Common.Models;

namespace Notification.Domain.Entity;

public class AppNotification : BaseEntity
{
    public AppNotification(
        string userId,
        NotificationType notificationType,
        DateTime creationDate,
        string description,
        string? notificationPartnerId,
        string? notificationPartnerEmail,
        string? notificationPartnerPhotoUrl,
        bool displayed = false
    )
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        NotificationType = notificationType;
        CreationDate = creationDate;
        Displayed = displayed;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        NotificationPartnerId = notificationPartnerId;
        NotificationPartnerEmail = notificationPartnerEmail;
        NotificationPartnerPhotoUrl = notificationPartnerPhotoUrl;
    }

    private AppNotification() { }

    public string UserId { get; private set; }
    public NotificationType NotificationType { get; private set; }
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
