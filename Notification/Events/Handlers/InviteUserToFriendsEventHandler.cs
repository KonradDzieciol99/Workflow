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
                    UserId = request.EventRecipient.UserId,
                    ObjectId = new BsonDocument { { "InviterUserId", friendInvitationId.InviterUserId }, { "InvitedUserId", friendInvitationId.InvitedUserId } },
                    EventType = request.EventType,
                    NotificationType = "FriendRequestReceived",
                    Description = $"You have received a friend request from {request.EventSender.UserEmail}",
                    // = JsonSerializer.Serialize(request),
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.EventSender
                };
                notificationForSender = new AppNotificationMongo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.EventSender.UserId,
                    ObjectId = new BsonDocument { { "InviterUserId", friendInvitationId.InviterUserId }, { "InvitedUserId", friendInvitationId.InvitedUserId } },
                    EventType = request.EventType,
                    NotificationType = "FriendRequestSent",
                    Description = $"You sent a friend request to {request.EventRecipient.UserEmail}",
                    //Data = JsonSerializer.Serialize(request),    
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.EventRecipient
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
                    UserId = request.EventRecipient.UserId,
                    ObjectId = request.ObjectId,
                    EventType = request.EventType,
                    NotificationType = "FriendRequestReceived",
                    Description = $"You have received a friend request from {request.EventSender.UserEmail}",
                    //Data = request,
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.EventSender
                }
            };
            var notificationEventForSender = new NotificationEvent()
            {
                AppNotification = new AppNotification()
                {
                    Id = notificationForSender.Id,
                    UserId = request.EventSender.UserId,
                    ObjectId = request.ObjectId,
                    EventType = request.EventType,
                    NotificationType = "FriendRequestSent",
                    Description = $"You sent a friend request to {request.EventRecipient.UserEmail}",
                    //Data = request,
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.EventRecipient
                }
            };

            await _azureServiceBusSender.PublishMessage(notificationEventForRecipient);
            await _azureServiceBusSender.PublishMessage(notificationEventForSender);

            return;
        }
    }
}
