using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
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
        var message = new MessageDto(request.Id,
                                     request.SenderId,
                                     request.SenderEmail,
                                     request.RecipientId,
                                     request.RecipientEmail,
                                     request.Content,
                                     request.DateRead,
                                     request.MessageSent);

        var groupName = GetGroupName(message.SenderEmail, message.RecipientEmail);
        var values = await _redisDb.HashValuesAsync(groupName);
        if (values.Contains(message.RecipientEmail))
        {
            message.DateRead = DateTime.UtcNow;
            var markChatMessageAsReadEvent = new MarkChatMessageAsReadEvent(message.Id, message.DateRead.Value);
            await _azureServiceBusSender.PublishMessage(markChatMessageAsReadEvent);
        }

        await _chatHubContext.Clients.Group(groupName).SendAsync("NewMessage", message, cancellationToken: cancellationToken);
        //await _presenceHubContext.Clients.User(message.RecipientId).SendAsync("NewMessageReceived", new { senderEmail = message.SenderEmail }, cancellationToken: cancellationToken);

        return;
    }
    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
