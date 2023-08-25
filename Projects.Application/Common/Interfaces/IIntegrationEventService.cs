using MessageBus;

namespace Projects.Application.Common.Interfaces
{
    public interface IIntegrationEventService
    {
        public void AddIntegrationEvent(IntegrationEvent domainEvent);
        public void RemoveIntegrationEvent(IntegrationEvent domainEvent);
        public Task PublishEventsThroughEventBusAsync();
    }
}
