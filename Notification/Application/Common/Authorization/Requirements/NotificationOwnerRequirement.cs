using Microsoft.AspNetCore.Authorization;

namespace Notification.Application.Common.Authorization.Requirements;

public class NotificationOwnerRequirement : IAuthorizationRequirement
{
    public NotificationOwnerRequirement(string notificationId)
    {
        AppNotificationId = notificationId ?? throw new ArgumentNullException(nameof(notificationId));
    }

    public string AppNotificationId { get; set; }
}
