using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using MediatR;
using MessageBus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBus;

public class AzureServiceBusConsumer : BackgroundService, IEventBusConsumer
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMediator _mediator;
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    private readonly AzureServiceBusConsumerOptions _options;
    private readonly ConcurrentDictionary<string, Type> _events;

    public AzureServiceBusConsumer(
        IServiceScopeFactory serviceScopeFactory,
        IMediator mediator,
        IOptions<AzureServiceBusConsumerOptions> options,
        ILogger<AzureServiceBusConsumer> logger
    )
    {
        this._serviceScopeFactory =
            serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
        ;
        this._events = new ConcurrentDictionary<string, Type>();
        RemoveAllRulesAsync().GetAwaiter().GetResult();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = new ServiceBusClient(_options.ServiceBusConnectionString);

        var BusProcessor = client.CreateProcessor(_options.TopicName, _options.SubscriptionName);
        BusProcessor.ProcessMessageAsync += EventHandlerAsync;
        BusProcessor.ProcessErrorAsync += ErrorHandler;
        await BusProcessor.StartProcessingAsync(stoppingToken);

        return;
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        _logger.LogError(
            ex,
            "ERROR handling integation event: {ExceptionMessage} - Context: {@ExceptionContext}",
            ex.Message,
            context
        );

        return Task.CompletedTask;
    }

    private async Task EventHandlerAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var type =
                _events[message.Subject]
                ?? throw new InvalidOperationException(
                    $"You did not subscribe to this event {message.Subject}"
                );

            MethodInfo sendAsyncMethod =
                this.GetType()
                    .GetMethod(nameof(SendAsync), BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException(
                    $"Something went wrong during execution {nameof(SendAsync)}"
                );

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
        var options = new JsonSerializerOptions { WriteIndented = true };

        var @event = JsonSerializer.Deserialize<T>(eventJSON, options);

        if (@event is null)
            throw new ArgumentNullException($"Message is empty{@event}");

        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(@event);
        return;
    }

    public async Task Subscribe<T>()
        where T : IntegrationEvent
    {
        var _administrationClient = new ServiceBusAdministrationClient(
            _options.ServiceBusConnectionString
        );

        var eventName = typeof(T).Name;

        try
        {
            await _administrationClient.CreateRuleAsync(
                _options.TopicName,
                _options.SubscriptionName,
                new CreateRuleOptions
                {
                    Filter = new CorrelationRuleFilter() { Subject = eventName },
                    Name = eventName
                }
            );
            _events.TryAdd(eventName, typeof(T));
        }
        catch (ServiceBusException ex)
            when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            //_logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
        }
        catch (Exception)
        {
            //_logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
            throw;
        }
        //}

        //_logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

        //_subsManager.AddSubscription<T, TH>();

        return;
    }

    private async Task RemoveAllRulesAsync()
    {
        var _administrationClient = new ServiceBusAdministrationClient(
            _options.ServiceBusConnectionString
        );

        try
        {
            var rules = _administrationClient.GetRulesAsync(
                _options.TopicName,
                _options.SubscriptionName
            );

            await foreach (var rule in rules)
            {
                await _administrationClient.DeleteRuleAsync(
                    _options.TopicName,
                    _options.SubscriptionName,
                    rule.Name
                );
            }
        }
        //catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessageNotFound || ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
        //{
        //    Console.WriteLine(ex.Message);
        //    //_logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
        //} to jest chyba wogole źle
        catch (Exception)
        {
            throw;
            //_logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.", RuleProperties.DefaultRuleName);
        }
        await Task.CompletedTask;

        return;
    }
}
