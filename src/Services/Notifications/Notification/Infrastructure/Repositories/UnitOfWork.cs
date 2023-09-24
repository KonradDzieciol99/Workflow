using Notification.Infrastructure.DataAccess;

namespace Notification.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _applicationDbContext;
    private bool disposed = false;

    public IAppNotificationRepository AppNotificationRepository =>
        new AppNotificationRepository(_applicationDbContext);

    public UnitOfWork(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext =
            applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
    }

    public async Task<bool> Complete()
    {
        return await _applicationDbContext.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        _applicationDbContext.ChangeTracker.DetectChanges();
        var changes = _applicationDbContext.ChangeTracker.HasChanges();

        return changes;
    }

    // Metoda zwalniająca zasoby.
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Zwalnianie zasobów zarządzanych (implementujących interfejs IDisposable).
                _applicationDbContext?.Dispose();
            }
            // Zwalnianie zasobów niezarządzanych.
            disposed = true;
        }
    }

    // Metoda publiczna wywoływana do zwolnienia zasobów.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
