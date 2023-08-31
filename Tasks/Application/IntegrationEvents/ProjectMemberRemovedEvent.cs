using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public class ProjectMemberRemovedEvent : IntegrationEvent
{
    public ProjectMemberRemovedEvent(string projectMemberId, string projectId, string userId)
    {
        ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    public string ProjectMemberId { get; set; }
    public string ProjectId { get; set; }
    public string UserId { get; set; }
}
public class ProjectMemberRemovedEventHandler : IRequestHandler<ProjectMemberRemovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectMemberRemovedEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task Handle(ProjectMemberRemovedEvent request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ProjectMemberRepository.ExecuteRemoveAsync(request.ProjectMemberId);

        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing a project member.");
    }
}
