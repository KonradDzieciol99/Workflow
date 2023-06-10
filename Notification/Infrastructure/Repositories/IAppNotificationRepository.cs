using Notification.Application.AppNotifications.Queries;
using Notification.Domain.Entity;

namespace Notification.Infrastructure.Repositories;

public interface IAppNotificationRepository : IRepository<AppNotification>
{
    Task<bool> CheckIfUserIsAOwnerOfAppNotification(string appNotificationId);
    Task<AppNotification?> GetAsync(string Id);
    Task<(List<AppNotification> AppNotifications, int totalCount)> GetAsync(string userId, GetAppNotificationsQuery @params);


}
