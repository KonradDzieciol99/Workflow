using System;

namespace MessageBus.Events;

public class TaskDeletedEvent : IntegrationEvent
{
    public TaskDeletedEvent(string id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public string Id { get; set; }
}