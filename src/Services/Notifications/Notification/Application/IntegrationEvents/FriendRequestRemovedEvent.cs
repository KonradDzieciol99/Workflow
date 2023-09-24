using MediatR;
using MessageBus;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents;

public record FriendRequestRemovedEvent(
    string ActionInitiatorUserId,
    string ActionInitiatorUserEmail,
    string? ActionInitiatorUserPhotoUrl,
    string FriendToRemoveUserId,
    string FriendToRemoveUserEmail,
    string? FriendToRemoveUserPhotoUrl
) : IntegrationEvent;

public class FriendRequestRemovedEventHandler : IRequestHandler<FriendRequestRemovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBusSender _azureServiceBusSender;

    public FriendRequestRemovedEventHandler(
        IUnitOfWork unitOfWork,
        IEventBusSender azureServiceBusSender
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _azureServiceBusSender =
            azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
        ;
    }

    public async Task Handle(FriendRequestRemovedEvent request, CancellationToken cancellationToken)
    {
        var AppNotifications = new List<AppNotification>();

        var oldNotifications =
            await _unitOfWork.AppNotificationRepository.GetByNotificationPartnersIdsAsync(
                request.ActionInitiatorUserId,
                request.FriendToRemoveUserId,
                new List<NotificationType>() { NotificationType.FriendRequestAccepted, }
            );

        var notificationForRemovedFriend = new AppNotification(
            request.FriendToRemoveUserId,
            NotificationType.InvitationDeclined,
            request.MessageCreated,
            $"User {request.ActionInitiatorUserEmail} removed you from friends",
            request.ActionInitiatorUserId,
            request.ActionInitiatorUserEmail,
            request.ActionInitiatorUserPhotoUrl
        );

        var notificationForActionInitiatorUser = new AppNotification(
            request.ActionInitiatorUserId,
            NotificationType.InvitationDeclinedByYou,
            request.MessageCreated,
            $"You have removed {request.FriendToRemoveUserEmail} from friends",
            request.FriendToRemoveUserId,
            request.FriendToRemoveUserEmail,
            request.FriendToRemoveUserPhotoUrl
        );

        AppNotifications.Add(notificationForRemovedFriend);
        AppNotifications.Add(notificationForActionInitiatorUser);

        _unitOfWork.AppNotificationRepository.AddRange(AppNotifications);
        _unitOfWork.AppNotificationRepository.RemoveRange(oldNotifications);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        var notificationEventForRemovedFriend = new NotificationAddedEvent(
            notificationForRemovedFriend.Id,
            notificationForRemovedFriend.UserId,
            (int)notificationForRemovedFriend.NotificationType,
            notificationForRemovedFriend.CreationDate,
            notificationForRemovedFriend.Displayed,
            notificationForRemovedFriend.Description,
            notificationForRemovedFriend.NotificationPartnerId,
            notificationForRemovedFriend.NotificationPartnerEmail,
            notificationForRemovedFriend.NotificationPartnerPhotoUrl,
            oldNotifications.Select(x => x.Id).ToList()
        );

        var notificationEventForActionInitiatorUser = new NotificationAddedEvent(
            notificationForActionInitiatorUser.Id,
            notificationForActionInitiatorUser.UserId,
            (int)notificationForActionInitiatorUser.NotificationType,
            notificationForActionInitiatorUser.CreationDate,
            notificationForActionInitiatorUser.Displayed,
            notificationForActionInitiatorUser.Description,
            notificationForActionInitiatorUser.NotificationPartnerId,
            notificationForActionInitiatorUser.NotificationPartnerEmail,
            notificationForActionInitiatorUser.NotificationPartnerPhotoUrl,
            oldNotifications.Select(x => x.Id).ToList()
        );

        await _azureServiceBusSender.PublishMessage(notificationEventForRemovedFriend);
        await _azureServiceBusSender.PublishMessage(notificationEventForActionInitiatorUser);

        return;
    }
}
