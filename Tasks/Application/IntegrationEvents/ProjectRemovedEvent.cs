using MediatR;
using MessageBus;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;


public class ProjectRemovedEvent : IntegrationEvent
{
    public ProjectRemovedEvent(string projectId, string name, string iconUrl)
    {
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IconUrl = iconUrl ?? throw new ArgumentNullException(nameof(iconUrl));
    }

    public string ProjectId { get; set; }
    public string Name { get; private set; }
    public string IconUrl { get; private set; }
}
public class ProjectRemovedEventHandler : IRequestHandler<ProjectRemovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectRemovedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(ProjectRemovedEvent request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ProjectMemberRepository.RemoveAllProjectMembersAsync(request.ProjectId);

        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing all project members.");

    }
}

