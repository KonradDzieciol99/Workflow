using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public record ProjectMemberAddedEvent(
    string ProjectMemberId,
    string UserId,
    string UserEmail,
    string? PhotoUrl,
    int Type,
    string ProjectId,
    int InvitationStatus,
    string ProjectName,
    string ProjectIconUrl,
    bool IsNewProjectCreator
) : IntegrationEvent;

public class ProjectMemberAddedEventHandler : IRequestHandler<ProjectMemberAddedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectMemberAddedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ProjectMemberAddedEvent request, CancellationToken cancellationToken)
    {
        var projectMember = new ProjectMember(
            request.ProjectMemberId,
            request.UserId,
            request.UserEmail,
            request.PhotoUrl,
            (ProjectMemberType)request.Type,
            (InvitationStatus)request.InvitationStatus,
            request.ProjectId
        );

        _unitOfWork.ProjectMemberRepository.Add(projectMember);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException("An error occurred while adding a project member.");

        await Task.CompletedTask;
        return;
    }
}
