﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tasks.Infrastructure.DataAccess;

namespace Tasks.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
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

    public async Task<List<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
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
