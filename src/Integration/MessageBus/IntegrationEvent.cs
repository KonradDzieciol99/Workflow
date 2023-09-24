using MediatR;
using System;

namespace MessageBus;

public abstract record IntegrationEvent : IRequest
{
    protected IntegrationEvent()
    {
        EventType = GetType().Name;
        MessageCreated = DateTime.UtcNow;
    }

    public string EventType { get; set; }
    public DateTime MessageCreated { get; set; }
}
