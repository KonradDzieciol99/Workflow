using Notification.Application.AppNotifications.Queries;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;

namespace Notification.Infrastructure.Repositories;

public interface IAppNotificationRepository : IRepository<AppNotification>
{
    Task<bool> CheckIfUserIsAOwnerOfAppNotification(string appNotificationId, string userId);
    Task<AppNotification?> GetAsync(string Id);
    Task<(List<AppNotification> AppNotifications, int totalCount)> GetAsync(string userId, GetAppNotificationsQuery @params);
    Task<List<string>> GetUnreadAsync(string userId);
    Task<List<AppNotification>> GetByNotificationPartnersIdsAsync(string NotificationPartnerIdOne, string NotificationPartnerIdTwo, List<NotificationType> notificationTypes);
}
