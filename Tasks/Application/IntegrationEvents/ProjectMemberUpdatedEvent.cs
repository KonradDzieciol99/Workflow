using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Domain.Common.Models;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public class ProjectMemberUpdatedEvent : IntegrationEvent
{
    public ProjectMemberUpdatedEvent(string? photoUrl, string projectMemberId, string userId, string userEmail, int type, int invitationStatus, string projectId)
    {
        PhotoUrl = photoUrl;
        this.ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        Type = type;
        InvitationStatus = invitationStatus;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    }

    public string? PhotoUrl { get; set; }
    public string ProjectMemberId { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public int Type { get; set; }
    public int InvitationStatus { get; set; }
    public string ProjectId { get; set; }
}

public class ProjectMemberUpdatedEventHandler : IRequestHandler<ProjectMemberUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectMemberUpdatedEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task Handle(ProjectMemberUpdatedEvent request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ProjectMemberRepository.ExecuteUpdateAsync(request.ProjectMemberId,
                                                                           (ProjectMemberType)request.Type
                                                                           , (InvitationStatus)request.InvitationStatus);

        //if (!await _unitOfWork.Complete())
        //    throw new InvalidOperationException("An error occurred while updating a project member.");
        if (result > 0)
        {
            await Task.CompletedTask;
            return;
        }

        throw new InvalidOperationException("An error occurred while removing a project member.");

        //await Task.CompletedTask;
        //return;
    }
}
