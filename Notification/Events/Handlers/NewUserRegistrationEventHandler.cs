using MediatR;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using MongoDB.Driver;
using System.Text.Json;

namespace Notification.Events.Handlers
{
    public class NewUserRegistrationEventHandler : IRequestHandler<NewUserRegistrationEvent>
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        public NewUserRegistrationEventHandler(IMongoDatabase mongoDatabase, IAzureServiceBusSender azureServiceBusSender)
        {
            this._mongoDatabase = mongoDatabase;
            this._azureServiceBusSender = azureServiceBusSender;
        }
        public async Task Handle(NewUserRegistrationEvent request, CancellationToken cancellationToken)
        {
            var collection = _mongoDatabase.GetCollection<AppNotification>("Notifications");

            AppNotification notification = new AppNotification()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.NotificationRecipient.UserId,
                ObjectId = request.ObjectId,
                EventType = request.EventType,
                NotificationType = "WelcomeNotification",
                Data = JsonSerializer.Serialize(request),
                Description = $"Thank you for registering {request.Email}, have fun testing!",
                CreationDate = request.MessageCreated,
                NotificationPartner = request.NotificationSender
            };
            await collection.InsertOneAsync(notification);

            var notificationEventForSender = new NotificationEvent()
            {
                AppNotification = notification,
            };

            await _azureServiceBusSender.PublishMessage(notificationEventForSender, "notification-queue");

            await Task.CompletedTask;

            return;
        }
    }
}
