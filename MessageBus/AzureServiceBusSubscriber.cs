using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using MediatR;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBus
{
    public class AzureServiceBusSubscriber : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMediator _mediator;
        private readonly ILogger<AzureServiceBusSubscriber> _logger;
        private readonly AzureServiceBusSubscriberOptions _options;
        private readonly string _topicName = "workflow_event_bus";
        private readonly string _subscriptionName;
        private readonly ConcurrentDictionary<string, Type> _events;

        public AzureServiceBusSubscriber(IServiceScopeFactory serviceScopeFactory,
                                         IMediator mediator,
                                         IOptions<AzureServiceBusSubscriberOptions> options,
                                         ILogger<AzureServiceBusSubscriber>  logger)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            this._mediator = mediator;
            this._logger = logger;
            this._options = options.Value;
            _options.Validate();
            //_topicName = topicName;
            _subscriptionName = _options.SubscriptionName;
            _events = new ConcurrentDictionary<string, Type>();
           RemoveAllRulesAsync().GetAwaiter().GetResult();
            // string topicName, string subscriptionName

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(_options.ServiceBusConnectionString);

            //if (_options.QueueNameAndEventTypePair is not null)
            //{
            //    foreach (var item in _options.QueueNameAndEventTypePair)
            //    {
            //var BusProcessor = client.CreateProcessor(item.Key);
            //BusProcessor.ProcessMessageAsync += (args) => EventHandlerAsync(args, _options.QueueNameAndEventTypePair);
            //BusProcessor.ProcessErrorAsync += ErrorHandler;
            //await BusProcessor.StartProcessingAsync();
            //    }
            //}

            //if (_options.TopicNameWithSubscriptionName is not null)
            //{
            //    foreach (var item in _options.TopicNameWithSubscriptionName)
            //    {
            //var BusProcessor = client.CreateProcessor(item.Key, item.Value);
            //BusProcessor.ProcessMessageAsync += (args) => EventHandlerAsync(args, _options.TopicNameAndEventTypePair);
            //BusProcessor.ProcessErrorAsync += ErrorHandler;
            //await BusProcessor.StartProcessingAsync();
            //    }
            //}

            var BusProcessor = client.CreateProcessor(_topicName, _subscriptionName);
            BusProcessor.ProcessMessageAsync += EventHandlerAsync;
            BusProcessor.ProcessErrorAsync += ErrorHandler;
            await BusProcessor.StartProcessingAsync();

            return;
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {

            var ex = args.Exception;
            var context = args.ErrorSource;

            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

        private async Task EventHandlerAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);
                //var label = args.Message.ApplicationProperties["Label"] as string;
                //if (label == null)
                //{
                //    throw new ArgumentNullException($"Label is empty: {args}");
                //}

                //var type = topicOrQueueNameWithTypeEvent[label];

                var type = _events[message.Subject];
                if (type == null)
                {
                    throw new ArgumentNullException("You did not subscribe to this event");
                }

                //throw new ArgumentNullException("test");
                //var type = _options.QueueNameAndEventTypePair[label];

                MethodInfo sendAsyncMethod = this.GetType().GetMethod(nameof(SendAsync), BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new ArgumentNullException("Something went wrong.");

                await (Task)sendAsyncMethod.MakeGenericMethod(type).Invoke(this, new object[] { body });

                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception)
            {

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    await args.CompleteMessageAsync(args.Message);
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

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var response = await mediator.Send(@event);
            }
            return;
        }

        //public async Task Subscribe<T, TH>()
        public async Task Subscribe<T>()
            //where T : IntegrationEvent
            //where TH : IIntegrationEventHandler<T>
        {

            var _administrationClient = new ServiceBusAdministrationClient(_options.ServiceBusConnectionString);

            var eventName = typeof(T).Name;

            //var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            //if (!containsKey)
            //{
                try
                {
                    await _administrationClient.CreateRuleAsync(_topicName, _subscriptionName, new CreateRuleOptions
                    {
                        Filter = new CorrelationRuleFilter() { Subject = eventName },
                        Name = eventName
                    });
                    _events.TryAdd(eventName, typeof(T));
                }
                catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                    //_logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
                }
                catch (Exception ex)
                {
                    //_logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
                    throw;
                }   
            //}

            //_logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            //_subsManager.AddSubscription<T, TH>();

            await Task.CompletedTask;

            return;
        }
        //private async Task RemoveDefaultRule()
        //{
        //    var _administrationClient = new ServiceBusAdministrationClient(_options.ServiceBusConnectionString);

        //    try
        //    {
        //        await _administrationClient.DeleteRuleAsync(_topicName, _subscriptionName, RuleProperties.DefaultRuleName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //        await Task.CompletedTask;

        //    return;
        //}
        private async Task RemoveAllRulesAsync()
        {
            var _administrationClient = new ServiceBusAdministrationClient(_options.ServiceBusConnectionString);

            try
            {
                 var rules =  _administrationClient.GetRulesAsync(_topicName, _subscriptionName);

                await foreach (var rule in rules)
                {
                    await _administrationClient.DeleteRuleAsync(_topicName, _subscriptionName, rule.Name);
                }
            }
            //catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessageNotFound || ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            //{
            //    Console.WriteLine(ex.Message);
            //    //_logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
            //} to jest chyba wogole źle
            catch (Exception ex) 
            {
                throw;
                //_logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.", RuleProperties.DefaultRuleName);
            }
            await Task.CompletedTask;

            return;
        }
    }
}
