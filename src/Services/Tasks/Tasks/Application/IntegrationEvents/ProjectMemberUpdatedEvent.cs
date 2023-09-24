using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Domain.Common.Models;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public record ProjectMemberUpdatedEvent(
    string? PhotoUrl,
    string ProjectMemberId,
    string UserId,
    string UserEmail,
    ProjectMemberType Type,
    InvitationStatus InvitationStatus,
    string ProjectId
) : IntegrationEvent;

public class ProjectMemberUpdatedEventHandler : IRequestHandler<ProjectMemberUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMemberUpdatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ProjectMemberUpdatedEvent request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ProjectMemberRepository.ExecuteUpdateAsync(
            request.ProjectMemberId,
            request.Type,
            request.InvitationStatus
        );
        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing a project member.");
    }
}
