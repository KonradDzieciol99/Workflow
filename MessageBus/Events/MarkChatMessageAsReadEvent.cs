using System;

namespace MessageBus.Events;

public class MarkChatMessageAsReadEvent : IntegrationEvent
{
    public string ChatMessageId { get; set; }
    public DateTime ChatMessageDateRead { get; set; }
}
