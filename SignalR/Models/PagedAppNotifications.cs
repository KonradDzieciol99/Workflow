namespace SignalR.Models;

public record PagedAppNotifications(List<AppNotification> AppNotifications, int TotalCount);
