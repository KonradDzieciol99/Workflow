using Tasks.Domain.Common.Exceptions;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Domain.Services;

public class AppTaskService : IAppTaskService
{
    private readonly IAppTaskRepository _appTaskRepository;

    public AppTaskService(IUnitOfWork unitOfWork)
    {
        _appTaskRepository = unitOfWork.AppTaskRepository;
    }

    public void RemoveAppTask(AppTask task, ProjectMember CurrentProjectMember)
    {
        if (
            CurrentProjectMember.Type != ProjectMemberType.Leader
            && CurrentProjectMember.Type != ProjectMemberType.Admin
            && task.TaskLeaderId != CurrentProjectMember.Id
        )
        {
            throw new TaskDomainException("User has no rights to delete this task");
        }

        _appTaskRepository.Remove(task);
    }
}
