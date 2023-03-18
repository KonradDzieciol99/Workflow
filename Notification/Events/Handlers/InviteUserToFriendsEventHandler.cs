using MessageBus.Events;
using MessageBus;
using MongoDB.Driver;
using System.Text.Json;
using MediatR;
using MessageBus.Models;
using MongoDB.Bson;
using Notification.Models;
using Microsoft.IdentityModel.Tokens;

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

            var ObjectIdAsString = JsonSerializer.Serialize(request.ObjectId);
            var friendInvitationId = JsonSerializer.Deserialize<FriendInvitationId>(ObjectIdAsString);
            AppNotificationMongo notificationForRecipient;
            AppNotificationMongo notificationForSender;
            List<AppNotificationMongo> notificationsArray = new List<AppNotificationMongo>();
            try
            {
                notificationForRecipient = new AppNotificationMongo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.NotificationRecipient.UserId,
                    ObjectId = new BsonDocument { { "InviterUserId", friendInvitationId.InviterUserId }, { "InvitedUserId", friendInvitationId.InvitedUserId } },
                    EventType = request.EventType,
                    NotificationType = "FriendRequestReceived",
                    Description = $"You have received a friend request from {request.NotificationSender.UserEmail}",
                    // = JsonSerializer.Serialize(request),
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.NotificationSender
                };
                notificationForSender = new AppNotificationMongo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.NotificationSender.UserId,
                    ObjectId = new BsonDocument { { "InviterUserId", friendInvitationId.InviterUserId }, { "InvitedUserId", friendInvitationId.InvitedUserId } },
                    EventType = request.EventType,
                    NotificationType = "FriendRequestSent",
                    Description = $"You sent a friend request to {request.NotificationRecipient.UserEmail}",
                    //Data = JsonSerializer.Serialize(request),    
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.NotificationRecipient
                };
                notificationsArray.Add(notificationForRecipient);
                notificationsArray.Add(notificationForSender);

                var collection = _mongoDatabase.GetCollection<AppNotificationMongo>("Notifications");
                await collection.InsertManyAsync(notificationsArray);

            }
            catch (Exception)
            {
                throw;
            }


            var notificationEventForRecipient = new NotificationEvent()
            {
                AppNotification = new AppNotification()
                {
                    Id = notificationForRecipient.Id,
                    UserId = request.NotificationRecipient.UserId,
                    ObjectId = request.ObjectId,
                    EventType = request.EventType,
                    NotificationType = "FriendRequestReceived",
                    Description = $"You have received a friend request from {request.NotificationSender.UserEmail}",
                    //Data = request,
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.NotificationSender
                }
            };
            var notificationEventForSender = new NotificationEvent()
            {
                AppNotification = new AppNotification()
                {
                    Id = notificationForSender.Id,
                    UserId = request.NotificationSender.UserId,
                    ObjectId = request.ObjectId,
                    EventType = request.EventType,
                    NotificationType = "FriendRequestSent",
                    Description = $"You sent a friend request to {request.NotificationRecipient.UserEmail}",
                    //Data = request,
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.NotificationRecipient
                }
            };

            await _azureServiceBusSender.PublishMessage(notificationEventForRecipient, "notification-queue");
            await _azureServiceBusSender.PublishMessage(notificationEventForSender, "notification-queue");

            return;
        }
    }
}
