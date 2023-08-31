using AutoMapper;
using MediatR;
using MessageBus;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.Repositories;

namespace Tasks.Application.IntegrationEvents;

public class ProjectMemberAddedEvent : IntegrationEvent
{
    public ProjectMemberAddedEvent(string projectMemberId, string userId, string userEmail, string? photoUrl, int type, string projectId, int invitationStatus, string projectName, string projectIconUrl, bool isNewProjectCreator)
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
        this.IsNewProjectCreator = isNewProjectCreator;
    }

    //public ProjectMemberAddedEvent(string projectMemberId, string userId, string userEmail, string? photoUrl, string projectId,int invitationStatus, int type = 2)
    //{
    //    ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
    //    UserId = userId ?? throw new ArgumentNullException(nameof(userId));
    //    UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
    //    PhotoUrl = photoUrl;
    //    Type = type;
    //    ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    //    InvitationStatus = invitationStatus;
    //}

    public string ProjectMemberId { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string? PhotoUrl { get; set; }
    public int Type { get; set; }
    public string ProjectId { get; set; }
    public int InvitationStatus { get; set; }
    public string ProjectName { get; set; }
    public string projectIconUrl { get; set; }
    public bool IsNewProjectCreator { get; set; }
}
public class ProjectMemberAddedEventHandler : IRequestHandler<ProjectMemberAddedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectMemberAddedEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task Handle(ProjectMemberAddedEvent request, CancellationToken cancellationToken)
    {

        var projectMember = new ProjectMember(request.ProjectMemberId,
                                              request.UserId,
                                              request.UserEmail,
                                              request.PhotoUrl,
                                              (ProjectMemberType)request.Type,
                                              (InvitationStatus)request.InvitationStatus,
                                              request.ProjectId);

        _unitOfWork.ProjectMemberRepository.Add(projectMember);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException("An error occurred while adding a project member.");

        await Task.CompletedTask;
        return;
    }
}
