using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class SendMessageToSignalREvent: IRequest
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientId { get; set; }
        public string RecipientEmail { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}
