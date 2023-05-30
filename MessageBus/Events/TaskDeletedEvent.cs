using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events;

public class TaskDeletedEvent: IntegrationEvent
{
    public TaskDeletedEvent(string id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public string Id { get; set; }
}