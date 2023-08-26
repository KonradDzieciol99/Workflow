using Microsoft.EntityFrameworkCore.Storage;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IReadOnlyProjectMemberRepository ReadOnlyProjectMemberRepository { get; }
    IProjectRepository ProjectRepository { get; }
    IReadOnlyProjectRepository ReadOnlyProjectRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
    bool HasActiveTransaction();
    Task<IDbContextTransaction?> BeginTransactionAsync();
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    void RollbackTransaction();
}
