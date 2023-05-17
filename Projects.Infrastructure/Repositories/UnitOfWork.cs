

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Projects.Application.Common.ServiceInterfaces;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Interfaces;
using Projects.Infrastructure.Common;
using Projects.Infrastructure.DataAccess;
using System.Threading;
using System.Transactions;

namespace Projects.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMediator _mediator;
        private bool disposed = false;
        public UnitOfWork(ApplicationDbContext applicationDbContext, IMediator mediator)
        {
            _applicationDbContext = applicationDbContext;
            _mediator = mediator;
        }

        public IReadOnlyProjectMemberRepository ProjectMemberRepository => new ReadOnlyProjectMemberRepository(_applicationDbContext);

        public IProjectRepository ProjectRepository => new ProjectRepository(_applicationDbContext);

        public IReadOnlyProjectRepository ReadOnlyProjectRepository => new ReadOnlyProjectRepository(_applicationDbContext);


        public async Task<bool> Complete()
        {
            await _mediator.DispatchDomainEventsAsync(_applicationDbContext);

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


        //public bool HasActiveTransaction()
        //{
        //    if (_applicationDbContext.Database.CurrentTransaction != null)
        //        return true;

        //    return false;
        //}
        private IDbContextTransaction _currentTransaction;

       // public bool HasActiveTransaction => _currentTransaction != null;
        public bool HasActiveTransaction()
        {
            return _currentTransaction != null;
        }
        public async Task<IDbContextTransaction?> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await _applicationDbContext.Database.BeginTransactionAsync();

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await _applicationDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                this.RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
