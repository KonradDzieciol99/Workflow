namespace Messenger.API.Repositories
{
    public interface IUnitOfWork
    {
        IMessageRepository MessageRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
