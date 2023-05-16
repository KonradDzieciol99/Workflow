namespace Projects.Repositories
{
    public interface IUnitOfWork
    {
        //IFriendInvitationRepository FriendInvitationRepository { get; }
        IProjectMemberRepository ProjectMemberRepository { get; }
        IProjectRepository ProjectRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
