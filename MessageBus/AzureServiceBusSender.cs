using Azure.Messaging.ServiceBus;
using MessageBus.Models;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageBus
{
    public class AzureServiceBusSender : IAzureServiceBusSender
    {
        private readonly AzureServiceBusSenderOptions _options;
        private readonly string _topicName = "workflow_event_bus";
        public AzureServiceBusSender(IOptions<AzureServiceBusSenderOptions> options)
        {
            this._options = options.Value;
        }
        public async Task PublishMessage(IntegrationEvent message)
        {
            //if (message.EventSender is null)
            //{
            //    //
            //} sprawdzić czy się wywali jeśli coś będzie nie ok z polami
            //message.Id=Guid.NewGuid().ToString();
            message.MessageCreated = DateTime.UtcNow;
            message.EventType = message.GetType().Name;

            await using var client = new ServiceBusClient(_options.ServiceBusConnectionString);

            ServiceBusSender sender = client.CreateSender(_topicName);

            string jsonMessage = JsonSerializer.Serialize(message, message.GetType());
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
                Subject = message.EventType
            };

            //finalMessage.ApplicationProperties["Label"] = queueOrTopicName;

            await sender.SendMessageAsync(finalMessage);

            //if (message is IUserPersistentNotification && message is not UserPersistentNotificationEvent)
            //{
            //    var userPersistentNotificationEvent = new UserPersistentNotificationEvent()
            //    {
            //        EventType = message.EventType,
            //        Id = message.Id,
            //        MessageCreated = message.MessageCreated,
            //        EventRecipient = message.EventRecipient,
            //        EventSender = message.EventSender,
            //        IsDisplayed = false
            //    };

            //    await client.DisposeAsync();

            //    await this.PublishMessage<UserPersistentNotificationEvent>(userPersistentNotificationEvent, "notification-queue");
            //}

            await client.DisposeAsync();
        }
    }
}
