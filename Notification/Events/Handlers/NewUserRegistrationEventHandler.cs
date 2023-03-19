using MediatR;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Notification.Models;
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
            var collection = _mongoDatabase.GetCollection<AppNotificationMongo>("Notifications");

            var ObjectIdAsString = JsonSerializer.Serialize(request.ObjectId);
            var stringId = JsonSerializer.Deserialize<string>(ObjectIdAsString);

            AppNotificationMongo notification = new AppNotificationMongo()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.NotificationRecipient.UserId,
                ObjectId = new BsonDocument { { "Id", stringId },},
                EventType = request.EventType,
                NotificationType = "WelcomeNotification",
                //Data = JsonSerializer.Serialize(request),
                Description = $"Thank you for registering {request.NotificationRecipient.UserEmail}, have fun testing!",
                CreationDate = request.MessageCreated,
                NotificationPartner = new SimpleUser("", "Workflow@Workflow.com", "https://res.cloudinary.com/ddmmg4wb2/image/upload/v1673385489/workflow-management_ssnvgy.jpg")
            };
            await collection.InsertOneAsync(notification);

            var notificationEventForSender = new NotificationEvent()
            {
                AppNotification = new AppNotification()
                {
                    Id = notification.Id,
                    UserId = request.NotificationRecipient.UserId,
                    ObjectId = request.ObjectId,
                    EventType = request.EventType,
                    NotificationType = "WelcomeNotification",
                    //Data = request,
                    Description = $"Thank you for registering {request.NotificationRecipient.UserEmail}, have fun testing!",
                    CreationDate = request.MessageCreated,
                    NotificationPartner = new SimpleUser("", "Workflow@Workflow.com", "https://res.cloudinary.com/ddmmg4wb2/image/upload/v1673385489/workflow-management_ssnvgy.jpg")
                }
        };

            await _azureServiceBusSender.PublishMessage(notificationEventForSender);

            await Task.CompletedTask;

            return;
        }
    }
}
