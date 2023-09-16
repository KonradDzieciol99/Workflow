﻿using Azure.Messaging.ServiceBus.Administration;
using Azure.Messaging.ServiceBus;
using MediatR;
using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text.Json;

namespace MessageBus;
public class RabbitMQConsumer : BackgroundService, IEventBusConsumer
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    private readonly RabbitMQConsumerOptions _options;
    private readonly ConcurrentDictionary<string, Type> _events;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConsumer(IServiceScopeFactory serviceScopeFactory,
                            IOptions<RabbitMQConsumerOptions> options,
                            ILogger<AzureServiceBusConsumer> logger)
    {
        this._serviceScopeFactory = serviceScopeFactory;
        this._logger = logger;
        this._options = options.Value;
        this._events = new ConcurrentDictionary<string, Type>();
        _connection = new ConnectionFactory { HostName = _options.Host, UserName = _options.UserName, Password = _options.Password, DispatchConsumersAsync = true }.CreateConnection();
        _channel = _connection.CreateModel();
        RemoveAllRulesAsync().GetAwaiter().GetResult();
        _channel.ExchangeDeclare(exchange: _options.Exchange, type: "direct");
        _channel.QueueDeclare(queue: _options.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += EventHandlerAsync;
        _channel.BasicConsume(queue: _options.Queue,
                             autoAck: false,
                             consumer: consumer);
        return;
    }
    private async Task EventHandlerAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var body = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {

            var type = _events[eventName];
            if (type == null)
            {
                throw new ArgumentNullException("You did not subscribe to this event");
            }

            MethodInfo sendAsyncMethod = this.GetType().GetMethod(nameof(SendAsync), BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new ArgumentNullException("Something went wrong.");

            await (Task)sendAsyncMethod.MakeGenericMethod(type).Invoke(this, new object[] { body });

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);

        }
        catch (Exception)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            else
                throw;
        }

        return;

    }
    private async Task SendAsync<T>(string eventJSON)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var @event = JsonSerializer.Deserialize<T>(eventJSON, options);

        if (@event is null)
            throw new ArgumentNullException($"Message is empty{@event}");

        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(@event);
        return;
    }

    public Task Subscribe<T>() where T : IntegrationEvent
    {
        var eventName = typeof(T).Name;

        _channel.QueueBind(queue: _options.Queue,
                           exchange: _options.Exchange,
                           routingKey: eventName);

        _events.TryAdd(eventName, typeof(T));

        return Task.CompletedTask;
    }


    private Task RemoveAllRulesAsync()
    {
        _channel.QueueDelete(queue: _options.Queue);
        return Task.CompletedTask;
    }
}