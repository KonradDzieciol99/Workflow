using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public record ProjectMemberDeclineInvitationEvent(
    string ProjectMemberId,
    string UserId,
    string UserEmail,
    string? PhotoUrl,
    int Type,
    string ProjectId,
    int InvitationStatus,
    string ProjectName,
    string ProjectIconUrl
) : IntegrationEvent;

public class ProjectMemberDeclineInvitationEventHandler
    : IRequestHandler<ProjectMemberDeclineInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMemberDeclineInvitationEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ProjectMemberDeclineInvitationEvent request,
        CancellationToken cancellationToken
    )
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
