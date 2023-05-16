using MediatR;
using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class NewOfflineUserEvent : IntegrationEvent, IRequest
    {
        public SimpleUser User { get; set; }
    }
}
