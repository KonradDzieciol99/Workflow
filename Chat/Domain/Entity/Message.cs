using Chat.Domain.Common.Models;
using System.Security.Cryptography;

namespace Chat.Domain.Entity;

public class Message : BaseEntity
{
    private Message(){}
    public Message(string senderId, string senderEmail, string recipientId, string recipientEmail, string content)
    {
        SenderId = senderId;
        SenderEmail = senderEmail;
        RecipientId = recipientId;
        RecipientEmail = recipientEmail;
        Content = content;
    }
    public string SenderId { get; private set; }
    public string SenderEmail { get; private set; }
    public string RecipientId { get; private set; }
    public string RecipientEmail { get; private set; }
    public string Content { get; private set; }
    public DateTime? DateRead { get; private set; }
    public DateTime MessageSent { get; private set; } = DateTime.Now;
    public bool SenderDeleted { get; private set; } = false;
    public bool RecipientDeleted { get; private set; } = false;

    public void MarkMessageAsRead()
    {
       this.DateRead = DateTime.UtcNow;
    }

}
