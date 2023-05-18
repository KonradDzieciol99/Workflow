using MediatR;
using MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Common.Interfaces
{
    public interface IIntegrationEventService
    {
        public void AddIntegrationEvent(IntegrationEvent domainEvent);
        public void RemoveIntegrationEvent(IntegrationEvent domainEvent);
        public Task PublishEventsThroughEventBusAsync();
    }
}
