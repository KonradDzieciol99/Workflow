namespace Chat.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        IFriendRequestRepository FriendRequestRepository { get; }
        IMessagesRepository MessagesRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
