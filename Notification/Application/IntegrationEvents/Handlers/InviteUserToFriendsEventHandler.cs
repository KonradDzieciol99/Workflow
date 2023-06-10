using MessageBus.Events;
using MediatR;
using MessageBus;
using Notification.Infrastructure.Repositories;
using Notification.Domain.Entity;
using Azure.Core;
using MongoDB.Bson;

namespace Notification.Application.IntegrationEvents.Handlers;

public class InviteUserToFriendsEventHandler : IRequestHandler<FriendInvitationAddedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public InviteUserToFriendsEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender)); ;
    }
    public async Task Handle(FriendInvitationAddedEvent request, CancellationToken cancellationToken)
    {

        var AppNotifications = new List<AppNotification>();

        var notificationForInvitationSendingUser = new AppNotification(request.InvitationSendingUserId,
                                                           request.ObjectId,
                                                           "FriendRequestSent",
                                                           request.MessageCreated,
                                                           $"You sent a friend request to {request.InvitedUserEmail}",
                                                           request.InvitedUserId,
                                                           request.InvitedUserEmail,
                                                           request.InvitedUserPhotoUrl);

        var notificationForInvitedUser = new AppNotification(request.InvitedUserId,
                                                           request.ObjectId,
                                                           "FriendRequestReceived",
                                                           request.MessageCreated,
                                                           $"You have received a friend request from {request.InvitationSendingUserEmail}",
                                                           request.InvitationSendingUserId,
                                                           request.InvitationSendingUserEmail,
                                                           request.InvitationSendingUserPhotoUrl);

        AppNotifications.Add(notificationForInvitationSendingUser);
        AppNotifications.Add(notificationForInvitedUser);

        _unitOfWork.AppNotificationRepository.AddRange(AppNotifications);

        if (await _unitOfWork.Complete()!)
            throw new InvalidOperationException();


        var notificationEventForInvitationSendingUser = new NotificationAddedEvent(notificationForInvitationSendingUser.Id,
                                                                  notificationForInvitationSendingUser.UserId,
                                                                  notificationForInvitationSendingUser.ObjectId,
                                                                  notificationForInvitationSendingUser.NotificationType,
                                                                  notificationForInvitationSendingUser.CreationDate,
                                                                  notificationForInvitationSendingUser.Displayed,
                                                                  notificationForInvitationSendingUser.Description,
                                                                  notificationForInvitationSendingUser.NotificationPartnerId,
                                                                  notificationForInvitationSendingUser.NotificationPartnerEmail,
                                                                  notificationForInvitationSendingUser.NotificationPartnerPhotoUrl
                                                                  );

        var notificationEventForInvitedUser = new NotificationAddedEvent(notificationForInvitedUser.Id,
                                                               notificationForInvitedUser.UserId,
                                                               notificationForInvitedUser.ObjectId,
                                                               notificationForInvitedUser.NotificationType,
                                                               notificationForInvitedUser.CreationDate,
                                                               notificationForInvitedUser.Displayed,
                                                               notificationForInvitedUser.Description,
                                                               notificationForInvitedUser.NotificationPartnerId,
                                                               notificationForInvitedUser.NotificationPartnerEmail,
                                                               notificationForInvitedUser.NotificationPartnerPhotoUrl);

        await _azureServiceBusSender.PublishMessage(notificationEventForInvitationSendingUser);
        await _azureServiceBusSender.PublishMessage(notificationEventForInvitedUser);

        return;
    }
}