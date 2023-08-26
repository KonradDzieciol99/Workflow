using MediatR;
using MessageBus;
using MessageBus.Events;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents.Handlers;

public class FriendRequestRemovedEventHandler : IRequestHandler<FriendRequestRemovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public FriendRequestRemovedEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender)); ;
    }
    public async Task Handle(FriendRequestRemovedEvent request, CancellationToken cancellationToken)
    {
        var AppNotifications = new List<AppNotification>();

        var oldNotifications = await _unitOfWork.AppNotificationRepository.GetByNotificationPartnersIdsAsync(
                    request.ActionInitiatorUserId,
                    request.FriendToRemoveUserId,
                    new List<NotificationType>()
                    {
                      NotificationType.FriendRequestAccepted,
                      //NotificationType.FriendRequestSent
                    });

        var notificationForRemovedFriend = new AppNotification(request.FriendToRemoveUserId,
            //request.ObjectId,
            NotificationType.InvitationDeclined,
            request.MessageCreated,
            $"User {request.ActionInitiatorUserEmail} removed you from friends",
            request.ActionInitiatorUserId,
            request.ActionInitiatorUserEmail,
            request.ActionInitiatorUserPhotoUrl);

        var notificationForActionInitiatorUser = new AppNotification(request.ActionInitiatorUserId,
            //request.ObjectId,
            NotificationType.InvitationDeclinedByYou,
            request.MessageCreated,
            $"You have removed {request.FriendToRemoveUserEmail} from friends",
            request.FriendToRemoveUserId,
            request.FriendToRemoveUserEmail,
            request.FriendToRemoveUserPhotoUrl);

        AppNotifications.Add(notificationForRemovedFriend);
        AppNotifications.Add(notificationForActionInitiatorUser);

        _unitOfWork.AppNotificationRepository.AddRange(AppNotifications);
        _unitOfWork.AppNotificationRepository.RemoveRange(oldNotifications);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();


        var notificationEventForRemovedFriend = new NotificationAddedEvent(notificationForRemovedFriend.Id,
                                                                       notificationForRemovedFriend.UserId,
                                                                       //notificationForRemovedFriend.ObjectId,
                                                                       (int)notificationForRemovedFriend.NotificationType,
                                                                       notificationForRemovedFriend.CreationDate,
                                                                       notificationForRemovedFriend.Displayed,
                                                                       notificationForRemovedFriend.Description,
                                                                       notificationForRemovedFriend.NotificationPartnerId,
                                                                       notificationForRemovedFriend.NotificationPartnerEmail,
                                                                       notificationForRemovedFriend.NotificationPartnerPhotoUrl,
                                                                       oldNotifications.Select(x => x.Id).ToList());

        var notificationEventForActionInitiatorUser = new NotificationAddedEvent(notificationForActionInitiatorUser.Id,
                                                                    notificationForActionInitiatorUser.UserId,
                                                                    //notificationForActionInitiatorUser.ObjectId,
                                                                    (int)notificationForActionInitiatorUser.NotificationType,
                                                                    notificationForActionInitiatorUser.CreationDate,
                                                                    notificationForActionInitiatorUser.Displayed,
                                                                    notificationForActionInitiatorUser.Description,
                                                                    notificationForActionInitiatorUser.NotificationPartnerId,
                                                                    notificationForActionInitiatorUser.NotificationPartnerEmail,
                                                                    notificationForActionInitiatorUser.NotificationPartnerPhotoUrl,
                                                                    oldNotifications.Select(x => x.Id).ToList());

        await _azureServiceBusSender.PublishMessage(notificationEventForRemovedFriend);
        await _azureServiceBusSender.PublishMessage(notificationEventForActionInitiatorUser);

        return;

    }
}
