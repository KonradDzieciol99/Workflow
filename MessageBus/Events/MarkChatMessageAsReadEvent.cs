using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events;

public class MarkChatMessageAsReadEvent : IntegrationEvent
{
    public string ChatMessageId { get; set; }
    public DateTime ChatMessageDateRead { get; set; }
}
