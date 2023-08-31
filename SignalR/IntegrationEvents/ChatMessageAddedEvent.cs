using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents;

public class ChatMessageAddedEvent : IntegrationEvent
{
    public ChatMessageAddedEvent(string id, string senderId, string senderEmail, string recipientId, string recipientEmail, string content, DateTime? dateRead, DateTime messageSent, bool senderDeleted, bool recipientDeleted)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        SenderId = senderId ?? throw new ArgumentNullException(nameof(senderId));
        SenderEmail = senderEmail ?? throw new ArgumentNullException(nameof(senderEmail));
        RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
        RecipientEmail = recipientEmail ?? throw new ArgumentNullException(nameof(recipientEmail));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        DateRead = dateRead;
        MessageSent = messageSent;
        SenderDeleted = senderDeleted;
        RecipientDeleted = recipientDeleted;
    }
    public string Id { get; set; }
    public string SenderId { get; set; }
    public string SenderEmail { get; set; }
    public string RecipientId { get; set; }
    public string RecipientEmail { get; set; }
    public string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
}
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
            request.DateRead = DateTime.UtcNow;
            var markChatMessageAsReadEvent = new MarkChatMessageAsReadEvent()
            {
                ChatMessageDateRead = (DateTime)request.DateRead,
                ChatMessageId = request.Id,
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
