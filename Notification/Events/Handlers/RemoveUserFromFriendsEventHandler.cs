﻿using MediatR;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Notification.Models;
using System.Text.Json;

namespace Notification.Events.Handlers
{
    public class RemoveUserFromFriendsEventHandler : IRequestHandler<RemoveUserFromFriendsEvent>
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        public RemoveUserFromFriendsEventHandler(IMongoClient mongoClient, IMongoDatabase mongoDatabase, IAzureServiceBusSender azureServiceBusSender)
        {
            this._mongoClient = mongoClient;
            this._mongoDatabase = mongoDatabase;
            this._azureServiceBusSender = azureServiceBusSender;
        }
        public async Task Handle(RemoveUserFromFriendsEvent request, CancellationToken cancellationToken)
        {
            try
            {
                var collection = _mongoDatabase.GetCollection<AppNotificationMongo>("Notifications");

                var ObjectIdAsString = JsonSerializer.Serialize(request.ObjectId);
                var friendInvitationId = JsonSerializer.Deserialize<FriendInvitationId>(ObjectIdAsString);

                AppNotificationMongo notificationForRecipient = new AppNotificationMongo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.EventRecipient.UserId,
                    ObjectId = new BsonDocument { { "InviterUserId", friendInvitationId.InviterUserId }, { "InvitedUserId", friendInvitationId.InvitedUserId } },
                    EventType = request.EventType,
                    NotificationType = "RemovedFromFriend",
                    Description = $"User {request.EventSender.UserEmail} removed you from friends",
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.EventSender
                };
                AppNotificationMongo notificationForSender = new AppNotificationMongo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.EventSender.UserId,
                    ObjectId = new BsonDocument { { "InviterUserId", friendInvitationId.InviterUserId }, { "InvitedUserId", friendInvitationId.InvitedUserId } },
                    EventType = request.EventType,
                    NotificationType = "YouDeletedFriend",
                    Description = $"You have removed {request.EventRecipient.UserEmail} from friends",
                    CreationDate = request.MessageCreated,
                    NotificationPartner = request.EventRecipient
                };

                var filter = Builders<AppNotificationMongo>.Filter.Eq(x => x.ObjectId["InvitedUserId"], friendInvitationId.InviterUserId) &
                              Builders<AppNotificationMongo>.Filter.Eq(x => x.ObjectId["InviterUserId"], friendInvitationId.InvitedUserId);

                var filter2 = Builders<AppNotificationMongo>.Filter.Eq(x => x.ObjectId["InviterUserId"], friendInvitationId.InviterUserId) &
                             Builders<AppNotificationMongo>.Filter.Eq(x => x.ObjectId["InvitedUserId"], friendInvitationId.InvitedUserId);

                var filter3 = Builders<AppNotificationMongo>.Filter.Eq(x => x.EventType, nameof(FriendInvitationAcceptedEvent));


                    var resoult = await collection.Find((filter | filter2) & filter3).ToListAsync();

                    var oldRecipientNotification = resoult.FirstOrDefault(x => x.UserId == request.EventRecipient.UserId);
                    if (oldRecipientNotification is not null)
                    {
                        notificationForRecipient.Id = oldRecipientNotification.Id;
                    }
                    var oldSenderNotification = resoult.FirstOrDefault(x => x.UserId == request.EventSender.UserId);
                    if (oldSenderNotification is not null)
                    {
                        notificationForSender.Id = oldSenderNotification.Id;
                    }

                    var replaceOptions = new ReplaceOptions { IsUpsert = false };
                    await collection.ReplaceOneAsync(x => x.Id == oldRecipientNotification.Id, notificationForRecipient, replaceOptions);
                    await collection.ReplaceOneAsync(x => x.Id == oldSenderNotification.Id, notificationForSender, replaceOptions);

                var notificationEventForRecipient = new NotificationEvent()
                {
                    AppNotification = new AppNotification()
                    {
                        Id = notificationForRecipient.Id,
                        UserId = request.EventRecipient.UserId,
                        ObjectId = request.ObjectId,
                        EventType = request.EventType,
                        NotificationType = notificationForRecipient.NotificationType,
                        Description = notificationForRecipient.Description,
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
                        NotificationType = notificationForSender.NotificationType,
                        Description = notificationForSender.Description,
                        CreationDate = request.MessageCreated,
                        NotificationPartner = request.EventRecipient
                    }
                };

                await _azureServiceBusSender.PublishMessage(notificationEventForRecipient);
                await _azureServiceBusSender.PublishMessage(notificationEventForSender);

                return;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}