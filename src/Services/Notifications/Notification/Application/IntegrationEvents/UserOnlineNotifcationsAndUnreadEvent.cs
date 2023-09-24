using MessageBus;
using Notification.Application.Common.Models;
using Notification.Domain.Entity;

namespace Notification.Application.IntegrationEvents;

public record UserOnlineNotifcationsAndUnreadEvent(
    UserDto OnlineUser,
    List<AppNotification> AppNotifications,
    int TotalCount,
    List<string> UnreadIds
) : IntegrationEvent;
