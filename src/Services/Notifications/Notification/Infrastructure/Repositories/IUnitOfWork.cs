namespace Notification.Infrastructure.Repositories;

public interface IUnitOfWork
{
    IAppNotificationRepository AppNotificationRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}
