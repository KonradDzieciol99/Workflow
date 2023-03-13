﻿using IdentityServer.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IdentityServer.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;

        public Repository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
        }
        public async Task<TEntity?> GetOneByIdAsync(params object?[]? keyValues)
        {
            return await _dbContext.Set<TEntity>().FindAsync(keyValues);
        }
        public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }
        public void Remove(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
        }
    }
}
