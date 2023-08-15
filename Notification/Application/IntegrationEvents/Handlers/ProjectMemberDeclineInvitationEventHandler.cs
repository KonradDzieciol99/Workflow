using MediatR;
using MessageBus.Events;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Infrastructure.Repositories;
using Notification.Domain.Entity;

namespace Notification.Application.IntegrationEvents.Handlers;

public class ProjectMemberDeclineInvitationEventHandler : IRequestHandler<ProjectMemberDeclineInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public ProjectMemberDeclineInvitationEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork;
        this._azureServiceBusSender = azureServiceBusSender;
    }
    public async Task Handle(ProjectMemberDeclineInvitationEvent request, CancellationToken cancellationToken)
    {
        var oldNotifications = await _unitOfWork.AppNotificationRepository.GetByNotificationPartnersIdsAsync(
                                request.UserId,
                                request.ProjectId,
                                new List<NotificationType>()
                                {
                                    NotificationType.InvitationToProjectRecived,
                                });

        var notification = new AppNotification(request.UserId,
                                               NotificationType.InvitationToProjectDeclined,
                                               request.MessageCreated,
                                               $"You declined an invitation from {request.ProjectName}",
                                               request.ProjectId,
                                               request.ProjectName,
                                               request.projectIconUrl);

        _unitOfWork.AppNotificationRepository.RemoveRange(oldNotifications);
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
                                               notification.NotificationPartnerPhotoUrl);

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}