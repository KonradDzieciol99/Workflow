namespace SignalR.Commons.Models;

public record PagedAppNotifications(List<AppNotification> AppNotifications, int TotalCount);
