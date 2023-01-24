using System.Linq.Expressions;

namespace Core.Interfaces.IRepositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FindOneAndIncludeAsync<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> navigationPropertyPath);
        Task<int> FindAndCountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetOneAsync(int id);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

    }
}
