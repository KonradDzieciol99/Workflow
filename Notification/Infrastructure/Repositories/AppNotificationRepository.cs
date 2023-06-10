using Notification.Infrastructure.DataAccess;
using Notification.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.Common.Extensions;
using Notification.Application.AppNotifications.Queries;

namespace Notification.Infrastructure.Repositories
{
    public class AppNotificationRepository : Repository<AppNotification>, IAppNotificationRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AppNotificationRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public Task<bool> CheckIfUserIsAOwnerOfAppNotification(string appNotificationId)
        {
            throw new NotImplementedException();
        }
        public async Task<AppNotification?> GetAsync(string Id)
        {
            return await _applicationDbContext.AppNotification.SingleOrDefaultAsync(x => x.Id == Id);
        }
        public async Task<(List<AppNotification> AppNotifications, int totalCount)> GetAsync(string userId, GetAppNotificationsQuery @params)
        {

            var query = _applicationDbContext.AppNotification.AsQueryable();

            if (string.IsNullOrWhiteSpace(@params.OrderBy) == false && @params.IsDescending.HasValue)
            {
                query.OrderBy(@params.OrderBy, @params.IsDescending.Value);
            }

            int totalCount = await query.CountAsync();

            var projects = await query.Skip(@params.Skip)
                                      .Take(@params.Take)
                                      .ToListAsync();

            return (projects, totalCount);
        }
    }
}
