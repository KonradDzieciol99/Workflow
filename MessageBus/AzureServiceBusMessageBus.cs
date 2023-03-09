
using Azure.Messaging.ServiceBus;
using MessageBus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        //can be improved
        //private string connectionString = "Endpoint=sb://workflowazureservicebus.servicebus.windows.net/;SharedAccessKeyName=AccessKey;SharedAccessKey=siRIzQcrn3bmCLkvdCklk/qFogTavWYhcMZQTtqB4j0=;EntityPath=newuserregister";
        private string connectionString = "Endpoint=sb://workflowazureservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=PLXZYECKYoA1ENmbkqz0TY4/eUaf6S7rFok7SczCaAs=";

        public async Task PublishMessage<T>(T message, string queueOrTopicName)
        {

            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(queueOrTopicName);

            string jsonMessage = JsonSerializer.Serialize(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            finalMessage.ApplicationProperties["Label"] = queueOrTopicName;

            await sender.SendMessageAsync(finalMessage);

            if (finalMessage is IUserPersistentNotification)
            {

                await sender.SendMessageAsync(finalMessage);
            }

            await client.DisposeAsync();
        }
    }
}
