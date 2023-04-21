using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Text.Json;

namespace SignalR.Common.PipelineBehaviour
{
    public class CollectUserEventsNotificationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IHubContext<MessagesHub> _messagesHubContext;
        private readonly IDatabase _redisDb;

        public CollectUserEventsNotificationBehaviour(IConnectionMultiplexer connectionMultiplexer,IHubContext<MessagesHub> messagesHubContext)
        {
            this._messagesHubContext = messagesHubContext;
            this._redisDb = connectionMultiplexer.GetDatabase();
        }//chyba nie potrzebne 
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is IUserPersistentNotification)
            {
                var message = request as BaseMessage;
                if (message is null)
                    return await next();

                var value = new HashEntry[]
                {
                    new HashEntry(Guid.NewGuid().ToString(), JsonSerializer.Serialize(message)),
                };
                await _redisDb.HashSetAsync($"user-notification-{message.EventRecipient.UserEmail}", value);
                await _redisDb.HashSetAsync($"user-notification-{message.EventSender.UserEmail}", value);

            }

            return await next();
        }
    }
}
