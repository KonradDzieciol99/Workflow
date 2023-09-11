using System.Linq.Expressions;

namespace Notification.Infrastructure.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task<TEntity?> GetOneByIdAsync(params object?[]? keyValues);
    Task<List<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> predicate);

}
