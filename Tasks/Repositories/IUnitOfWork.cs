namespace Tasks.Repositories
{
    public interface IUnitOfWork
    {
        IProjectMemberRepository ProjectMemberRepository { get; }
        IAppTaskRepository AppTaskRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
