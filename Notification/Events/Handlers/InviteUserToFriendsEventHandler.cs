using MessageBus.Events;
using MessageBus;
using MongoDB.Driver;
using System.Text.Json;
using MediatR;
using MessageBus.Models;

namespace Notification.Events.Handlers
{
    public class InviteUserToFriendsEventHandler : IRequestHandler<InviteUserToFriendsEvent>
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        public InviteUserToFriendsEventHandler(IMongoDatabase mongoDatabase, IAzureServiceBusSender azureServiceBusSender)
        {
            this._mongoDatabase = mongoDatabase;
            this._azureServiceBusSender = azureServiceBusSender;
        }
        public async Task Handle(InviteUserToFriendsEvent request, CancellationToken cancellationToken)
        {

            var collection = _mongoDatabase.GetCollection<AppNotification>("Notifications");

            AppNotification notificationForRecipient = new AppNotification()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.NotificationRecipient.UserId,
                ObjectId = request.ObjectId,
                EventType = request.EventType,
                NotificationType = "FriendRequestReceived",
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
                NotificationType = "FriendRequestSent",
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
