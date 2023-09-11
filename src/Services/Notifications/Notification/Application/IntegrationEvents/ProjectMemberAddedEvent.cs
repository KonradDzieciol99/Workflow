using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

public record ProjectMemberAddedEvent(string ProjectMemberId, string UserId, string UserEmail, string? PhotoUrl, int Type, string ProjectId, int InvitationStatus, string ProjectName, string projectIconUrl, bool IsNewProjectCreator) : IntegrationEvent;

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