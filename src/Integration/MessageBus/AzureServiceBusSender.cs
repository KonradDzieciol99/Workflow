using Azure.Messaging.ServiceBus;
using MessageBus.Models;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageBus;

public class AzureServiceBusSender : IEventBusSender
{
    private readonly AzureServiceBusSenderOptions _options;
    public AzureServiceBusSender(IOptions<AzureServiceBusSenderOptions> options)
    {
        this._options = options.Value;
    }
    public async Task PublishMessage(IntegrationEvent message)
    {
        await using var client = new ServiceBusClient(_options.ServiceBusConnectionString);

        var sender = client.CreateSender(_options.TopicName);

        var jsonMessage = JsonSerializer.Serialize(message, message.GetType());
        var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Subject = message.EventType
        };

        await sender.SendMessageAsync(finalMessage);

        await client.DisposeAsync();
    }
}
