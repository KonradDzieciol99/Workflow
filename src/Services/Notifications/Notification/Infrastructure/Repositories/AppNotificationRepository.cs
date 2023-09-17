using Microsoft.EntityFrameworkCore;
using Notification.Application.AppNotifications.Queries;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Common.Extensions;
using Notification.Infrastructure.DataAccess;

namespace Notification.Infrastructure.Repositories;

public class AppNotificationRepository : Repository<AppNotification>, IAppNotificationRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public AppNotificationRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
    }

    public async Task<bool> CheckIfUserIsAOwnerOfAppNotification(string appNotificationId, string userId)
    {
        return await _applicationDbContext.AppNotification.AnyAsync(x => x.Id == appNotificationId && x.UserId == userId);

    }
    public async Task<AppNotification?> GetAsync(string Id)
    {
        return await _applicationDbContext.AppNotification.SingleOrDefaultAsync(x => x.Id == Id);
    }
    public async Task<List<AppNotification>> GetByNotificationPartnersIdsAsync(string NotificationPartnerIdOne, string NotificationPartnerIdTwo, List<NotificationType> notificationTypes)
    {
        return await _applicationDbContext.AppNotification.Where(x => ((x.UserId == NotificationPartnerIdOne && x.NotificationPartnerId == NotificationPartnerIdTwo) ||
                                                                      (x.UserId == NotificationPartnerIdTwo && x.NotificationPartnerId == NotificationPartnerIdOne)) &&
                                                                      notificationTypes.Contains(x.NotificationType)
                                                           ).ToListAsync();
    }
    public async Task<(List<AppNotification> AppNotifications, int totalCount)> GetAsync(string userId, GetAppNotificationsQuery @params)
    {

        var query = _applicationDbContext.AppNotification.AsQueryable();

        if (string.IsNullOrWhiteSpace(@params.OrderBy) == false && @params.IsDescending.HasValue)
        {
            query.OrderBy(@params.OrderBy, @params.IsDescending.Value);
        }

        query = query.Where(n => n.UserId == userId);

        int totalCount = await query.CountAsync();

        var projects = await query.Skip(@params.Skip)
                                  .Take(@params.Take)
                                  .ToListAsync();

        return (projects, totalCount);
    }
    public async Task<List<string>> GetUnreadAsync(string userId)
    {
        return await _applicationDbContext.AppNotification.Where(n => n.UserId == userId && n.Displayed == false)
                                             .Select(n => n.Id)
                                             .ToListAsync();
    }
}
