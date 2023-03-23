using Mango.MessageBus;
using MediatR;
using MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace SignalR.Events.Handlers
{
    public class SendMessageToSignalREventHandler : IRequestHandler<SendMessageToSignalREvent>
    {
        private readonly IDatabase _redisDb;
        private readonly IAzureServiceBusSender _azureServiceBusSender;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IHubContext<PresenceHub> _presenceHubContext;

        public SendMessageToSignalREventHandler(IConnectionMultiplexer connectionMultiplexer
            , IAzureServiceBusSender azureServiceBusSender,
            IHubContext<ChatHub> chatHubContext,
            IHubContext<PresenceHub> presenceHubContext,
            IHubContext<MessagesHub> messagesHubContext
            )
        {
            this._redisDb = connectionMultiplexer.GetDatabase();
            this._azureServiceBusSender = azureServiceBusSender;
            this._chatHubContext = chatHubContext;
            this._presenceHubContext = presenceHubContext;
        }
        public async Task Handle(SendMessageToSignalREvent request, CancellationToken cancellationToken)
        {

            var groupName = GetGroupName(request.SenderEmail, request.RecipientEmail);

            var values = await _redisDb.HashValuesAsync(groupName);
            if (values.Contains(request.RecipientEmail))
            {
                request.DateRead = DateTime.UtcNow;
                var markChatMessageAsReadEvent = new MarkChatMessageAsReadEvent() 
                {
                    DateRead = (DateTime)request.DateRead,
                    ObjectId = request.ObjectId,
                };
                await _azureServiceBusSender.PublishMessage(markChatMessageAsReadEvent);
            }

            await _chatHubContext.Clients.Group(groupName).SendAsync("NewMessage", request);
            await _presenceHubContext.Clients.User(request.RecipientId).SendAsync("NewMessageReceived", new { senderEmail = request.SenderEmail });

            return;
        }
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
