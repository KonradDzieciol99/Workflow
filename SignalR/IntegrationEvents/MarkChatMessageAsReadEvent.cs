using MessageBus;

namespace SignalR.IntegrationEvents;

public class MarkChatMessageAsReadEvent : IntegrationEvent
{
    public string ChatMessageId { get; set; }
    public DateTime ChatMessageDateRead { get; set; }
}