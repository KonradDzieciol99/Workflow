using Azure.Messaging.ServiceBus;
using MessageBus.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageBus;
public class RabbitMQSender : IEventBusSender
{
    private readonly RabbitMQSenderOptions _options;

    public RabbitMQSender(IOptions<RabbitMQSenderOptions> options)
    {
        this._options = options.Value;
    }

    public Task PublishMessage(IntegrationEvent message)
    {
        var factory = new ConnectionFactory { Uri=new Uri(_options.RabbitMQConnectionString), DispatchConsumersAsync = true };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        //channel.QueueDeclare(queue: "hello",
        //                     durable: false,
        //                     exclusive: false,
        //                     autoDelete: false,
        //                     arguments: null);

        var jsonMessage = JsonSerializer.Serialize(message, message.GetType());
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        channel.BasicPublish(exchange: _options.Exchange,
                             routingKey: message.EventType,
                             body: body);
        
        return Task.CompletedTask;
    }
}
