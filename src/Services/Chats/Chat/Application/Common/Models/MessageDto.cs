namespace Chat.Application.Common.Models;

public class MessageDto
{
    public required string Id { get; set; }
    public required string SenderId { get; set; }
    public required string SenderEmail { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientEmail { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public required DateTime MessageSent { get; set; }
}
