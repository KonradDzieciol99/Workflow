﻿using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using MessageBus.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageBus
{
    public class AzureServiceBusSender: IAzureServiceBusSender
    {
        private readonly AzureServiceBusSenderOptions _options;
        public AzureServiceBusSender(IOptions<AzureServiceBusSenderOptions> options)
        {
            this._options = options.Value;
        }
        public async Task PublishMessage<T>(T message, string queueOrTopicName) where T : BaseMessage
        {
            //if (message.NotificationSender is null)
            //{
            //    //
            //} sprawdzić czy się wywali jeśli coś będzie nie ok z polami

            message.Id=Guid.NewGuid().ToString();
            message.MessageCreated = DateTime.UtcNow;

            await using var client = new ServiceBusClient(_options.ServiceBusConnectionString);

            ServiceBusSender sender = client.CreateSender(queueOrTopicName);

            string jsonMessage = JsonSerializer.Serialize(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            finalMessage.ApplicationProperties["Label"] = queueOrTopicName;

            await sender.SendMessageAsync(finalMessage);

            await client.DisposeAsync();
        }
    }
}
