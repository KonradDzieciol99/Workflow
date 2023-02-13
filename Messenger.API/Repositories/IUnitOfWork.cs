namespace Socjal.API.Repositories
{
    public interface IUnitOfWork
    {
        IMessageRepository MessageRepository { get; }
        IUserRepository UserRepository { get; }
        IFriendInvitationRepository FriendInvitationRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
