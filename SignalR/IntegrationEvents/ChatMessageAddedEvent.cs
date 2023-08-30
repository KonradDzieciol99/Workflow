using MessageBus;

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
