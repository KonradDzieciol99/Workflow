using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

public record FriendRequestAcceptedEvent(
    string InvitationSendingUserId,
    string InvitationSendingUserEmail,
    string? InvitationSendingUserPhotoUrl,
    string InvitationAcceptingUserId,
    string InvitationAcceptingUserEmail,
    string? InvitationAcceptingUserPhotoUrl
) : IntegrationEvent;

public class FriendRequestAcceptedEventHandler : IRequestHandler<FriendRequestAcceptedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBusSender _azureServiceBusSender;

    public FriendRequestAcceptedEventHandler(
        IUnitOfWork unitOfWork,
        IEventBusSender azureServiceBusSender
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _azureServiceBusSender =
            azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
        ;
    }

    public async Task Handle(
        FriendRequestAcceptedEvent request,
        CancellationToken cancellationToken
    )
    {
        var AppNotifications = new List<AppNotification>();

        var oldNotifications =
            await _unitOfWork.AppNotificationRepository.GetByNotificationPartnersIdsAsync(
                request.InvitationSendingUserId,
                request.InvitationAcceptingUserId,
                new List<NotificationType>()
                {
                    NotificationType.FriendRequestReceived,
                    NotificationType.FriendRequestSent
                }
            );

        var notificationForInvitationSendingUser = new AppNotification(
            request.InvitationSendingUserId,
            NotificationType.FriendRequestAccepted,
            request.MessageCreated,
            $"You and {request.InvitationAcceptingUserEmail} are friends now!",
            request.InvitationAcceptingUserId,
            request.InvitationAcceptingUserEmail,
            request.InvitationAcceptingUserPhotoUrl
        );

        var notificationForInvitationAcceptingUser = new AppNotification(
            request.InvitationAcceptingUserId,
            NotificationType.FriendRequestAccepted,
            request.MessageCreated,
            $"You and {request.InvitationSendingUserEmail} are friends now!",
            request.InvitationSendingUserId,
            request.InvitationSendingUserEmail,
            request.InvitationSendingUserPhotoUrl
        );

        AppNotifications.Add(notificationForInvitationSendingUser);
        AppNotifications.Add(notificationForInvitationAcceptingUser);

        _unitOfWork.AppNotificationRepository.RemoveRange(oldNotifications);
        _unitOfWork.AppNotificationRepository.AddRange(AppNotifications);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        var notificationEventForRecipient = new NotificationAddedEvent(
            notificationForInvitationSendingUser.Id,
            notificationForInvitationSendingUser.UserId,
            (int)notificationForInvitationSendingUser.NotificationType,
            notificationForInvitationSendingUser.CreationDate,
            notificationForInvitationSendingUser.Displayed,
            notificationForInvitationSendingUser.Description,
            notificationForInvitationSendingUser.NotificationPartnerId,
            notificationForInvitationSendingUser.NotificationPartnerEmail,
            notificationForInvitationSendingUser.NotificationPartnerPhotoUrl,
            oldNotifications.Select(x => x.Id).ToList()
        );

        var notificationEventForSender = new NotificationAddedEvent(
            notificationForInvitationAcceptingUser.Id,
            notificationForInvitationAcceptingUser.UserId,
            (int)notificationForInvitationAcceptingUser.NotificationType,
            notificationForInvitationAcceptingUser.CreationDate,
            notificationForInvitationAcceptingUser.Displayed,
            notificationForInvitationAcceptingUser.Description,
            notificationForInvitationAcceptingUser.NotificationPartnerId,
            notificationForInvitationAcceptingUser.NotificationPartnerEmail,
            notificationForInvitationAcceptingUser.NotificationPartnerPhotoUrl,
            oldNotifications.Select(x => x.Id).ToList()
        );

        await _azureServiceBusSender.PublishMessage(notificationEventForRecipient);
        await _azureServiceBusSender.PublishMessage(notificationEventForSender);

        return;
    }
}
