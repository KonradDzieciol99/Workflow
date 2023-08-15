﻿using MediatR;
using MessageBus;
using MessageBus.Events;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents.Handlers;

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
                                               notification.NotificationPartnerPhotoUrl);

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
//TODO
//var notification = new AppNotification(request.UserId,
//                                       NotificationType.WelcomeNotification,
//                                       request.MessageCreated,
//                                       $"Thank you for registering {request.Email}, have fun testing!",
//                                       "Workflow",
//                                       null,
//                                       null);

//_unitOfWork.AppNotificationRepository.Add(notification);

//if (!await _unitOfWork.Complete())
//    throw new InvalidOperationException();

//var @event = new NotificationAddedEvent(notification.Id,
//                                       notification.UserId,
//                                       (int)notification.NotificationType,
//                                       notification.CreationDate,
//                                       notification.Displayed,
//                                       notification.Description,
//                                       notification.NotificationPartnerId,
//                                       notification.NotificationPartnerEmail,
//                                       notification.NotificationPartnerPhotoUrl);

//await _azureServiceBusSender.PublishMessage(@event);

//return;