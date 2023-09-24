using MessageBus;

namespace SignalR.IntegrationEvents;

public record MarkChatMessageAsReadEvent(string ChatMessageId, DateTime ChatMessageDateRead)
    : IntegrationEvent;
