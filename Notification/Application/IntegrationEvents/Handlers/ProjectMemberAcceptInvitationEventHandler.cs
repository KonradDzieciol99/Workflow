﻿using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents.Handlers;

public class ProjectMemberAcceptInvitationEventHandler : IRequestHandler<ProjectMemberAcceptInvitationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public ProjectMemberAcceptInvitationEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork;
        this._azureServiceBusSender = azureServiceBusSender;
    }
    public async Task Handle(ProjectMemberAcceptInvitationEvent request, CancellationToken cancellationToken)
    {
        var oldNotifications = await _unitOfWork.AppNotificationRepository.GetByNotificationPartnersIdsAsync(
                                request.UserId,
                                request.ProjectId,
                                new List<NotificationType>()
                                {
                                    NotificationType.InvitationToProjectRecived,
                                });

        var notification = new AppNotification(request.UserId,
                                               NotificationType.InvitationToProjectAccepted,
                                               request.MessageCreated,
                                               $"you have accepted an invitation to {request.ProjectName}",
                                               request.ProjectId,
                                               request.ProjectName,
                                               request.projectIconUrl,
                                               true);

        _unitOfWork.AppNotificationRepository.RemoveRange(oldNotifications);
        _unitOfWork.AppNotificationRepository.Add(notification);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        //var @event = new NotificationAddedEvent(notification.Id,
        //                                       notification.UserId,
        //                                       (int)notification.NotificationType,
        //                                       notification.CreationDate,
        //                                       notification.Displayed,
        //                                       notification.Description,
        //                                       notification.NotificationPartnerId,
        //                                       notification.NotificationPartnerEmail,
        //                                       notification.NotificationPartnerPhotoUrl,
        //                                       oldNotifications.Select(x => x.Id).ToList());

        //await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
