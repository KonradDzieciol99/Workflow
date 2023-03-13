using System.Linq.Expressions;

namespace IdentityServer.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetOneByIdAsync(params object?[]? keyValues);
    }
}
