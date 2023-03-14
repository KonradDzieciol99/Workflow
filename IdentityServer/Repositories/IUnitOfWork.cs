namespace IdentityServer.Repositories
{
    public interface IUnitOfWork
    {
        IIdentityUserRepository IdentityUserRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
