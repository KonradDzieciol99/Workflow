﻿using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

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
    private readonly IEventBusSender _azureServiceBusSender;

    public ProjectMemberAddedEventHandler(
        IUnitOfWork unitOfWork,
        IEventBusSender azureServiceBusSender
    )
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._azureServiceBusSender =
            azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
    }

    public async Task Handle(ProjectMemberAddedEvent request, CancellationToken cancellationToken)
    {
        if (request.IsNewProjectCreator)
            return;

        var notification = new AppNotification(
            request.UserId,
            NotificationType.InvitationToProjectRecived,
            request.MessageCreated,
            $"you have been invited to a {request.ProjectName}",
            request.ProjectId,
            request.ProjectName,
            request.ProjectIconUrl
        );

        _unitOfWork.AppNotificationRepository.Add(notification);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        var @event = new NotificationAddedEvent(
            notification.Id,
            notification.UserId,
            (int)notification.NotificationType,
            notification.CreationDate,
            notification.Displayed,
            notification.Description,
            notification.NotificationPartnerId,
            notification.NotificationPartnerEmail,
            notification.NotificationPartnerPhotoUrl,
            null
        );

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
