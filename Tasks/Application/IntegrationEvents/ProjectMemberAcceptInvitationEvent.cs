using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Domain.Common.Models;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public class ProjectMemberAcceptInvitationEvent : IntegrationEvent
{
    public ProjectMemberAcceptInvitationEvent(string projectMemberId, string userId, string userEmail, string? photoUrl, int type, string projectId, int invitationStatus, string projectName, string projectIconUrl)
    {
        ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        PhotoUrl = photoUrl;
        Type = type;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        InvitationStatus = invitationStatus;
        ProjectName = projectName ?? throw new ArgumentNullException(nameof(projectName));
        this.projectIconUrl = projectIconUrl ?? throw new ArgumentNullException(nameof(projectIconUrl));
    }

    public string ProjectMemberId { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string? PhotoUrl { get; set; }
    public int Type { get; set; }
    public string ProjectId { get; set; }
    public int InvitationStatus { get; set; }
    public string ProjectName { get; set; }
    public string projectIconUrl { get; set; }
}
public class ProjectMemberAcceptInvitationEventHandler : IRequestHandler<ProjectMemberAcceptInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectMemberAcceptInvitationEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        //if (!await _unitOfWork.Complete())
        //    throw new InvalidOperationException("An error occurred while updating a project member.");

        //await Task.CompletedTask;
        //return;
    }
}
