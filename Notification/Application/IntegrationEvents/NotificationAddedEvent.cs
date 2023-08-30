using MessageBus;

namespace Notification.Application.IntegrationEvents;


public class NotificationAddedEvent : IntegrationEvent
{

    //public NotificationAddedEvent(string id, string userId, /*object? objectId,*/ string notificationType, DateTime creationDate, bool displayed, string description, string? notificationPartnerUserId, string? notificationPartnerUserEmail, string? notificationPartnerUserPhotoUrl)
    //{
    //    Id = id ?? throw new ArgumentNullException(nameof(id));
    //    UserId = userId ?? throw new ArgumentNullException(nameof(userId));
    //    //ObjectId = objectId;
    //    NotificationType = notificationType ?? throw new ArgumentNullException(nameof(notificationType));
    //    CreationDate = creationDate;
    //    Displayed = displayed;
    //    Description = description ?? throw new ArgumentNullException(nameof(description));
    //    NotificationPartnerUserId = notificationPartnerUserId;
    //    NotificationPartnerUserEmail = notificationPartnerUserEmail;
    //    NotificationPartnerUserPhotoUrl = notificationPartnerUserPhotoUrl;
    //}

    //public string Id { get; private set; }
    //public string UserId { get; private set; }
    ////public object? ObjectId { get; private set; }

    //public string NotificationType { get; private set; }
    //public DateTime CreationDate { get; private set; }
    //public bool Displayed { get; private set; } 
    //public string Description { get; private set; }

    //public string? NotificationPartnerUserId { get; set; }
    //public string? NotificationPartnerUserEmail { get; set; }
    //public string? NotificationPartnerUserPhotoUrl { get; set; }

    public NotificationAddedEvent(string id, string userId, int notificationType, DateTime creationDate, bool displayed, string description, string? notificationPartnerId, string? notificationPartnerEmail, string? notificationPartnerPhotoUrl, List<string>? oldNotificationsIds)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        NotificationType = notificationType;
        CreationDate = creationDate;
        Displayed = displayed;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        NotificationPartnerId = notificationPartnerId;
        NotificationPartnerEmail = notificationPartnerEmail;
        NotificationPartnerPhotoUrl = notificationPartnerPhotoUrl;
        OldNotificationsIds = oldNotificationsIds;
    }

    public string Id { get; set; }
    public string UserId { get; set; }
    public int NotificationType { get; set; }
    public DateTime CreationDate { get; set; }
    public bool Displayed { get; set; }
    public string Description { get; set; }
    public string? NotificationPartnerId { get; set; }
    public string? NotificationPartnerEmail { get; set; }
    public string? NotificationPartnerPhotoUrl { get; set; }
    public List<string>? OldNotificationsIds { get; set; }
}
