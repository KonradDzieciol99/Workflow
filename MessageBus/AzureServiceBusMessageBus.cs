
using Azure.Messaging.ServiceBus;
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
        private string connectionString = "Endpoint=sb://workflowazureservicebus.servicebus.windows.net/;SharedAccessKeyName=AccessKey;SharedAccessKey=siRIzQcrn3bmCLkvdCklk/qFogTavWYhcMZQTtqB4j0=;EntityPath=newuserregister";

        public async Task PublishMessage<T>(T message, string topicName)
        {

            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topicName);

            string jsonMessage = JsonSerializer.Serialize(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);

            await client.DisposeAsync();
        }
    }
}
