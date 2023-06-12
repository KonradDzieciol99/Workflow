using MediatR;
using MessageBus;
using MessageBus.Events;
//using MessageBus.Models;
using MongoDB.Bson;
using Notification.Domain.Entity;
using Notification.Infrastructure.Repositories;
using System.Text.Json;

namespace Notification.Application.IntegrationEvents.Handlers
{
    public class FriendInvitationAcceptedEventHandler : IRequestHandler<FriendRequestAcceptedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        public FriendInvitationAcceptedEventHandler(IUnitOfWork unitOfWork, IAzureServiceBusSender azureServiceBusSender)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender)); ;
        }
        public async Task Handle(FriendRequestAcceptedEvent request, CancellationToken cancellationToken)
        {

            var AppNotifications = new List<AppNotification>();

            var notificationForInvitationSendingUser = new AppNotification(request.InvitationSendingUserId,
                                                               //request.ObjectId,
                                                               "NewFriendAdded",
                                                               request.MessageCreated,
                                                               $"You and {request.InvitationAcceptingUserEmail} are friends now!",
                                                               request.InvitationAcceptingUserId,
                                                               request.InvitationAcceptingUserEmail,
                                                               request.InvitationAcceptingUserPhotoUrl);

            var notificationForInvitationAcceptingUser = new AppNotification(request.InvitationAcceptingUserId,
                                                               //request.ObjectId,
                                                               "NewFriendAdded",
                                                               request.MessageCreated,
                                                               $"You and {request.InvitationSendingUserEmail} are friends now!",
                                                               request.InvitationSendingUserId,
                                                               request.InvitationSendingUserEmail,
                                                               request.InvitationSendingUserPhotoUrl);

            AppNotifications.Add(notificationForInvitationSendingUser);
            AppNotifications.Add(notificationForInvitationAcceptingUser);

            _unitOfWork.AppNotificationRepository.AddRange(AppNotifications);

            if (await _unitOfWork.Complete()!)
                throw new InvalidOperationException();


            var notificationEventForRecipient = new NotificationAddedEvent(notificationForInvitationSendingUser.Id,
                                                                      notificationForInvitationSendingUser.UserId,
                                                                      //notificationForInvitationSendingUser.ObjectId,
                                                                      notificationForInvitationSendingUser.NotificationType,
                                                                      notificationForInvitationSendingUser.CreationDate,
                                                                      notificationForInvitationSendingUser.Displayed,
                                                                      notificationForInvitationSendingUser.Description,
                                                                      notificationForInvitationSendingUser.NotificationPartnerId,
                                                                      notificationForInvitationSendingUser.NotificationPartnerEmail,
                                                                      notificationForInvitationSendingUser.NotificationPartnerPhotoUrl
                                                                      );

            var notificationEventForSender = new NotificationAddedEvent(notificationForInvitationAcceptingUser.Id,
                                                                   notificationForInvitationAcceptingUser.UserId,
                                                                   //notificationForInvitationAcceptingUser.ObjectId,
                                                                   notificationForInvitationAcceptingUser.NotificationType,
                                                                   notificationForInvitationAcceptingUser.CreationDate,
                                                                   notificationForInvitationAcceptingUser.Displayed,
                                                                   notificationForInvitationAcceptingUser.Description,
                                                                   notificationForInvitationAcceptingUser.NotificationPartnerId,
                                                                   notificationForInvitationAcceptingUser.NotificationPartnerEmail,
                                                                   notificationForInvitationAcceptingUser.NotificationPartnerPhotoUrl);

            await _azureServiceBusSender.PublishMessage(notificationEventForRecipient);
            await _azureServiceBusSender.PublishMessage(notificationEventForSender);

            return;
        }

    }
}
