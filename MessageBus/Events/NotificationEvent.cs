using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NotificationEvent : IntegrationEvent
    {
        public AppNotification AppNotification { get; set; }

    }
}
