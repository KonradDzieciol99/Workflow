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
    private readonly IEventBusSender _azureServiceBusSender;
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly IHubContext<PresenceHub> _presenceHubContext;

    public ChatMessageAddedEventHandler(IConnectionMultiplexer connectionMultiplexer
        , IEventBusSender azureServiceBusSender,
        IHubContext<ChatHub> chatHubContext,
        IHubContext<PresenceHub> presenceHubContext
        )
    {
        _redisDb = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
        _chatHubContext = chatHubContext ?? throw new ArgumentNullException(nameof(chatHubContext));
        _presenceHubContext = presenceHubContext ?? throw new ArgumentNullException(nameof(presenceHubContext));
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

        await _chatHubContext.Clients.Group(groupName).SendAsync("NewMessage", request, cancellationToken: cancellationToken);
        await _presenceHubContext.Clients.User(request.RecipientId).SendAsync("NewMessageReceived", new { senderEmail = request.SenderEmail }, cancellationToken: cancellationToken);

        return;
    }
    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
