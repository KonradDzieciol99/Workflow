using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents;

public record ChatMessageAddedEvent(string Id, string SenderId, string SenderEmail, string RecipientId, string RecipientEmail, string Content, DateTime? DateRead, DateTime MessageSent, bool SenderDeleted, bool RecipientDeleted) : IntegrationEvent;
public class ChatMessageAddedEventHandler : IRequestHandler<ChatMessageAddedEvent>
{
    private readonly IDatabase _redisDb;
    private readonly IAzureServiceBusSender _azureServiceBusSender;
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public ChatMessageAddedEventHandler(IConnectionMultiplexer connectionMultiplexer
        , IAzureServiceBusSender azureServiceBusSender,
        IHubContext<ChatHub> chatHubContext,
        IHubContext<PresenceHub> presenceHubContext,
        IHubContext<MessagesHub> messagesHubContext
        )
    {
        _redisDb = connectionMultiplexer.GetDatabase();
        _azureServiceBusSender = azureServiceBusSender;
        _chatHubContext = chatHubContext;
        _presenceHubContext = presenceHubContext;
    }
    public async Task Handle(ChatMessageAddedEvent request, CancellationToken cancellationToken)
    {

        var groupName = GetGroupName(request.SenderEmail, request.RecipientEmail);

        var values = await _redisDb.HashValuesAsync(groupName);
        if (values.Contains(request.RecipientEmail))
        {
            var markChatMessageAsReadEvent = new MarkChatMessageAsReadEvent(request.Id, DateTime.UtcNow);
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
