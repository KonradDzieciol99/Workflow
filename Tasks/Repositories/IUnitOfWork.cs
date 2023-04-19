namespace Tasks.Repositories
{
    public interface IUnitOfWork
    {
        IAppTaskRepository AppTaskRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
