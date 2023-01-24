using Application.Interfaces;
using Application.Interfaces.IRepositories;

namespace Core.Interfaces.IRepositories
{
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        IRefreshTokensRepository RefreshTokensRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
