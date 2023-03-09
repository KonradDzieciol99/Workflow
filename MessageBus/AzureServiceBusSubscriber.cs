using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using MediatR;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace MessageBus
{
    public class AzureServiceBusSubscriber : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMediator _mediator;
        private readonly AzureServiceBusSubscriberOptions _options;

        public AzureServiceBusSubscriber(IServiceScopeFactory serviceScopeFactory, IMediator mediator, IOptions<AzureServiceBusSubscriberOptions> options)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            this._mediator = mediator;
            this._options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(_options.ServiceBusConnectionString);

            if (_options.QueueNameAndEventTypePair is not null)
            {
                foreach (var item in _options.QueueNameAndEventTypePair)
                {
                    var BusProcessor = client.CreateProcessor(item.Key);
                    BusProcessor.ProcessMessageAsync += (args) => EventHandlerAsync(args, _options.QueueNameAndEventTypePair);
                    BusProcessor.ProcessErrorAsync += ErrorHandler;
                    await BusProcessor.StartProcessingAsync();
                }
            }

            if (_options.TopicNameWithSubscriptionName is not null)
            {
                foreach (var item in _options.TopicNameWithSubscriptionName)
                {
                    var BusProcessor = client.CreateProcessor(item.Key, item.Value);
                    BusProcessor.ProcessMessageAsync += (args) => EventHandlerAsync(args, _options.TopicNameAndEventTypePair);
                    BusProcessor.ProcessErrorAsync += ErrorHandler;
                    await BusProcessor.StartProcessingAsync();
                }
            }

            return;
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task EventHandlerAsync(ProcessMessageEventArgs args, Dictionary<string, Type> topicOrQueueNameWithTypeEvent)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var label = args.Message.ApplicationProperties["Label"] as string;
            if (label == null)
            {
                throw new ArgumentNullException($"Label is empty: {args}");
            }

            var type = topicOrQueueNameWithTypeEvent[label];
            //var type = _options.QueueNameAndEventTypePair[label];

            MethodInfo sendAsyncMethod = this.GetType().GetMethod(nameof(SendAsync), BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new ArgumentNullException("Something went wrong.");

            sendAsyncMethod.MakeGenericMethod(type).Invoke(this, new object[] { body });

            await args.CompleteMessageAsync(args.Message);

            return;
        }
        private async Task SendAsync<T>(string eventJSON)
        {
            var decodedEvent = JsonSerializer.Deserialize<T>(eventJSON);

            if (decodedEvent is null) { throw new ArgumentNullException($"Message is empty{decodedEvent}"); }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var response = await mediator.Send(decodedEvent);
            }
            return;
        }
    }
}
