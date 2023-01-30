namespace Socjal.API.Dto
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string SenderEmail { get; set; }
        public int RecipientId { get; set; }//
        public string RecipientEmail { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        public string SenderPhotoUrl { get; set; }//
        public string RecipientPhotoUrl { get; set; }//
    }
}
