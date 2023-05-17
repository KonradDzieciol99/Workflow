using Microsoft.EntityFrameworkCore.Storage;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Interfaces;

namespace Projects.Application.Common.ServiceInterfaces;

public interface IUnitOfWork
{
    IReadOnlyProjectMemberRepository ProjectMemberRepository { get; }
    IProjectRepository ProjectRepository { get; }
    IReadOnlyProjectRepository ReadOnlyProjectRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
    bool HasActiveTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    void RollbackTransaction();
}
