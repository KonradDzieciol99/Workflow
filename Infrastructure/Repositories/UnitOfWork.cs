
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Core.Interfaces.IRepositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private Hashtable _repositories;

        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }

        public IRefreshTokensRepository RefreshTokensRepository => new RefreshTokensRepository(_applicationDbContext);

        public async Task<bool> Complete()
        {
            return (await _applicationDbContext.SaveChangesAsync() > 0);
        }
        public bool HasChanges()
        {
            _applicationDbContext.ChangeTracker.DetectChanges();
            var changes = _applicationDbContext.ChangeTracker.HasChanges();

            return changes;
        }
        public void Dispose()
        {
            _applicationDbContext.Dispose();
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<TEntity>(_applicationDbContext);
                _repositories.Add(type, repositoryInstance);
            }

            var repository = _repositories[type];

            if (repository is IGenericRepository<TEntity> GenericRepositoryWithType)
            {
                return GenericRepositoryWithType;
            }

            throw new ArgumentNullException("can't find IGenericRepository<TEntity> in collection");
        }
    }
}
