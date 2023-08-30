using MessageBus;
using Notification.Application.Common.Models;
using Notification.Domain.Entity;

namespace Notification.Application.IntegrationEvents;

public class UserOnlineNotifcationsAndUnreadEvent : IntegrationEvent
{
    public UserOnlineNotifcationsAndUnreadEvent(UserDto onlineUser, List<AppNotification> appNotifications, int totalCount, List<string> unreadIds)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
        AppNotifications = appNotifications ?? throw new ArgumentNullException(nameof(appNotifications));
        TotalCount = totalCount;
        UnreadIds = unreadIds ?? throw new ArgumentNullException(nameof(unreadIds));
    }

    public UserDto OnlineUser { get; set; }
    public List<AppNotification> AppNotifications { get; set; }
    public int TotalCount { get; set; }
    public List<string> UnreadIds { get; set; }
}
