using Mango.MessageBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class MarkChatMessageAsReadEvent : BaseMessage, IRequest
    {
        public int Id { get; set; }
        public DateTime DateRead { get; set; }
    }
}
