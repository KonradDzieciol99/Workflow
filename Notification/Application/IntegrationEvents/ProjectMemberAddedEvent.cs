using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

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
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public ProjectMemberAddedEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork;
        this._azureServiceBusSender = azureServiceBusSender;
    }
    public async Task Handle(ProjectMemberAddedEvent request, CancellationToken cancellationToken)
    {
        if (request.IsNewProjectCreator)
            return;


        var notification = new AppNotification(request.UserId,
                                               NotificationType.InvitationToProjectRecived,
                                               request.MessageCreated,
                                               $"you have been invited to a {request.ProjectName}",
                                               request.ProjectId,
                                               request.ProjectName,
                                               request.projectIconUrl);

        _unitOfWork.AppNotificationRepository.Add(notification);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        var @event = new NotificationAddedEvent(notification.Id,
                                               notification.UserId,
                                               (int)notification.NotificationType,
                                               notification.CreationDate,
                                               notification.Displayed,
                                               notification.Description,
                                               notification.NotificationPartnerId,
                                               notification.NotificationPartnerEmail,
                                               notification.NotificationPartnerPhotoUrl,
                                               null);

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}