using MessageBus;
using SignalR.Models;

namespace SignalR.IntegrationEvents;

public class UserOnlineNotifcationsAndUnreadEvent : IntegrationEvent
{
    public UserDto OnlineUser { get; set; }
    public List<AppNotification> AppNotifications { get; set; }
    public int TotalCount { get; set; }
    public List<string> UnreadIds { get; set; }

}
