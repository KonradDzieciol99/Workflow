using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public record ProjectMemberRemovedEvent(string ProjectMemberId, string ProjectId, string UserId)
    : IntegrationEvent;

public class ProjectMemberRemovedEventHandler : IRequestHandler<ProjectMemberRemovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMemberRemovedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ProjectMemberRemovedEvent request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ProjectMemberRepository.ExecuteRemoveAsync(
            request.ProjectMemberId
        );

        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing a project member.");
    }
}
