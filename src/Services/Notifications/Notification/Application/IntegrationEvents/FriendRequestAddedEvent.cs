﻿using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

public record FriendRequestAddedEvent(
    string InvitationSendingUserId,
    string InvitationSendingUserEmail,
    string? InvitationSendingUserPhotoUrl,
    string InvitedUserId,
    string InvitedUserEmail,
    string? InvitedUserPhotoUrl
) : IntegrationEvent;

public class FriendRequestAddedEventHandler : IRequestHandler<FriendRequestAddedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBusSender _azureServiceBusSender;

    public FriendRequestAddedEventHandler(
        IUnitOfWork unitOfWork,
        IEventBusSender azureServiceBusSender
    )
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._azureServiceBusSender =
            azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
        ;
    }

    public async Task Handle(FriendRequestAddedEvent request, CancellationToken cancellationToken)
    {
        var AppNotifications = new List<AppNotification>();

        var notificationForInvitationSendingUser = new AppNotification(
            request.InvitationSendingUserId,
            NotificationType.FriendRequestSent,
            request.MessageCreated,
            $"You sent a friend request to {request.InvitedUserEmail}",
            request.InvitedUserId,
            request.InvitedUserEmail,
            request.InvitedUserPhotoUrl
        );

        var notificationForInvitedUser = new AppNotification(
            request.InvitedUserId,
            NotificationType.FriendRequestReceived,
            request.MessageCreated,
            $"You have received a friend request from {request.InvitationSendingUserEmail}",
            request.InvitationSendingUserId,
            request.InvitationSendingUserEmail,
            request.InvitationSendingUserPhotoUrl
        );

        AppNotifications.Add(notificationForInvitationSendingUser);
        AppNotifications.Add(notificationForInvitedUser);

        _unitOfWork.AppNotificationRepository.AddRange(AppNotifications);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        var notificationEventForInvitationSendingUser = new NotificationAddedEvent(
            notificationForInvitationSendingUser.Id,
            notificationForInvitationSendingUser.UserId,
            (int)notificationForInvitationSendingUser.NotificationType,
            notificationForInvitationSendingUser.CreationDate,
            notificationForInvitationSendingUser.Displayed,
            notificationForInvitationSendingUser.Description,
            notificationForInvitationSendingUser.NotificationPartnerId,
            notificationForInvitationSendingUser.NotificationPartnerEmail,
            notificationForInvitationSendingUser.NotificationPartnerPhotoUrl,
            null
        );

        var notificationEventForInvitedUser = new NotificationAddedEvent(
            notificationForInvitedUser.Id,
            notificationForInvitedUser.UserId,
            (int)notificationForInvitedUser.NotificationType,
            notificationForInvitedUser.CreationDate,
            notificationForInvitedUser.Displayed,
            notificationForInvitedUser.Description,
            notificationForInvitedUser.NotificationPartnerId,
            notificationForInvitedUser.NotificationPartnerEmail,
            notificationForInvitedUser.NotificationPartnerPhotoUrl,
            null
        );

        await _azureServiceBusSender.PublishMessage(notificationEventForInvitationSendingUser);
        await _azureServiceBusSender.PublishMessage(notificationEventForInvitedUser);

        return;
    }
}
