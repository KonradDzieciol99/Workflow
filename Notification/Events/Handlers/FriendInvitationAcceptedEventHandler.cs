using Mango.MessageBus;
using MediatR;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Notification.Models;
using System.Text.Json;

namespace Notification.Events.Handlers
{
    public class FriendInvitationAcceptedEventHandler : IRequestHandler<FriendInvitationAcceptedEvent>
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        public FriendInvitationAcceptedEventHandler(IMongoDatabase mongoDatabase, IAzureServiceBusSender azureServiceBusSender)
        {
            this._mongoDatabase = mongoDatabase;
            this._azureServiceBusSender = azureServiceBusSender;
        }
        public async Task Handle(FriendInvitationAcceptedEvent request, CancellationToken cancellationToken)
        {

            var collection = _mongoDatabase.GetCollection<AppNotification>("Notifications");

            AppNotification notificationForRecipient = new AppNotification()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.NotificationRecipient.UserId,
                ObjectId = request.ObjectId,
                EventType = request.EventType,
                NotificationType = "NewFriendAdded",
                Data = JsonSerializer.Serialize(request),
                CreationDate = request.MessageCreated,
                NotificationPartner = request.NotificationSender
            };
            AppNotification notificationForSender = new AppNotification()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.NotificationSender.UserId,
                ObjectId = request.ObjectId,
                EventType = request.EventType,
                NotificationType = "NewFriendAdded",
                Data = JsonSerializer.Serialize(request),
                CreationDate = request.MessageCreated,
                NotificationPartner = request.NotificationRecipient
            };
            AppNotification[] notificationsArray = { notificationForRecipient, notificationForSender };

            await collection.InsertManyAsync(notificationsArray);

            var notificationEventForRecipient = new NotificationEvent()
            {
                AppNotification = notificationForRecipient,
            };
            var notificationEventForSender = new NotificationEvent()
            {
                AppNotification = notificationForSender,
            };

            await _azureServiceBusSender.PublishMessage(notificationEventForRecipient, "notification-queue");
            await _azureServiceBusSender.PublishMessage(notificationEventForSender, "notification-queue");

            return;
        }
    }
}
