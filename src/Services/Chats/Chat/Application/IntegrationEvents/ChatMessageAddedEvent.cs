using MessageBus;

namespace Chat.Application.IntegrationEvents;

public record ChatMessageAddedEvent(string Id, string SenderId, string SenderEmail, string RecipientId, string RecipientEmail, string Content, DateTime? DateRead, DateTime MessageSent, bool SenderDeleted, bool RecipientDeleted) : IntegrationEvent;