using Tasks.Domain.Entity;

namespace Tasks.Domain.Services;

public interface IAppTaskService
{
    void RemoveAppTask(AppTask task, ProjectMember CurrentProjectMember);
}
