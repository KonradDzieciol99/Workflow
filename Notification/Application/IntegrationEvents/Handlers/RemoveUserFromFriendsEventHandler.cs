using MediatR;
using MessageBus;
using MessageBus.Events;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;

namespace Notification.Application.IntegrationEvents.Handlers
{
    public class RemoveUserFromFriendsEventHandler : IRequestHandler<FriendInvitationRemovedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        public RemoveUserFromFriendsEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender)); ;
        }
        public async Task Handle(FriendInvitationRemovedEvent request, CancellationToken cancellationToken)
        {
            var AppNotifications = new List<AppNotification>();

            var notificationForRemovedFriend = new AppNotification(request.FriendToRemoveUserId,
                //request.ObjectId,
                "RemovedFromFriend",
                request.MessageCreated,
                $"User {request.ActionInitiatorUserEmail} removed you from friends",
                request.ActionInitiatorUserId,
                request.ActionInitiatorUserEmail,
                request.ActionInitiatorUserPhotoUrl);

            var notificationForActionInitiatorUser = new AppNotification(request.ActionInitiatorUserId,
                //request.ObjectId,
                "YouDeletedFriend",
                request.MessageCreated,
                $"You have removed {request.FriendToRemoveUserEmail} from friends",
                request.FriendToRemoveUserId,
                request.FriendToRemoveUserEmail,
                request.FriendToRemoveUserPhotoUrl);

            AppNotifications.Add(notificationForRemovedFriend);
            AppNotifications.Add(notificationForActionInitiatorUser);

            _unitOfWork.AppNotificationRepository.AddRange(AppNotifications);

            if (await _unitOfWork.Complete()!)
                throw new InvalidOperationException();


            var notificationEventForRemovedFriend = new NotificationAddedEvent(notificationForRemovedFriend.Id,
                                                                           notificationForRemovedFriend.UserId,
                                                                           //notificationForRemovedFriend.ObjectId,
                                                                           notificationForRemovedFriend.NotificationType,
                                                                           notificationForRemovedFriend.CreationDate,
                                                                           notificationForRemovedFriend.Displayed,
                                                                           notificationForRemovedFriend.Description,
                                                                           notificationForRemovedFriend.NotificationPartnerId,
                                                                           notificationForRemovedFriend.NotificationPartnerEmail,
                                                                           notificationForRemovedFriend.NotificationPartnerPhotoUrl);

            var notificationEventForActionInitiatorUser = new NotificationAddedEvent(notificationForActionInitiatorUser.Id,
                                                                        notificationForActionInitiatorUser.UserId,
                                                                        //notificationForActionInitiatorUser.ObjectId,
                                                                        notificationForActionInitiatorUser.NotificationType,
                                                                        notificationForActionInitiatorUser.CreationDate,
                                                                        notificationForActionInitiatorUser.Displayed,
                                                                        notificationForActionInitiatorUser.Description,
                                                                        notificationForActionInitiatorUser.NotificationPartnerId,
                                                                        notificationForActionInitiatorUser.NotificationPartnerEmail,
                                                                        notificationForActionInitiatorUser.NotificationPartnerPhotoUrl);

            await _azureServiceBusSender.PublishMessage(notificationEventForRemovedFriend);
            await _azureServiceBusSender.PublishMessage(notificationEventForActionInitiatorUser);

            return;

        }
    }
}
