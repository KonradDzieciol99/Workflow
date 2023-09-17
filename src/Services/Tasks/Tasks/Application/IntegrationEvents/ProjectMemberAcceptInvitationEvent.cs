using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Domain.Common.Models;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public record ProjectMemberAcceptInvitationEvent(string ProjectMemberId, string UserId, string UserEmail, string? PhotoUrl, int Type, string ProjectId, int InvitationStatus, string ProjectName, string ProjectIconUrl) : IntegrationEvent;
public class ProjectMemberAcceptInvitationEventHandler : IRequestHandler<ProjectMemberAcceptInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    public ProjectMemberAcceptInvitationEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(ProjectMemberAcceptInvitationEvent request, CancellationToken cancellationToken)
    {

        var result = await _unitOfWork.ProjectMemberRepository.ExecuteUpdateAsync(request.ProjectMemberId,
                                                                           (ProjectMemberType)request.Type,
                                                                           (InvitationStatus)request.InvitationStatus);

        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing a project member.");
    }
}
