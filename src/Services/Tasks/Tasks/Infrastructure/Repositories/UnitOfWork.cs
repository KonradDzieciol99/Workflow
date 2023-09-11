﻿using Tasks.Infrastructure.DataAccess;

namespace Tasks.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _applicationDbContext;
    private bool disposed = false;

    public IAppTaskRepository AppTaskRepository => new AppTaskRepository(_applicationDbContext);
    public IProjectMemberRepository ProjectMemberRepository => new ProjectMemberRepository(_applicationDbContext);
    public UnitOfWork(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
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
                if (_applicationDbContext != null)
                {
                    _applicationDbContext.Dispose();
                }
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