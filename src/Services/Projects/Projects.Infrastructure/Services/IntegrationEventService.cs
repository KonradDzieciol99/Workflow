using MessageBus;
using Projects.Application.Common.Interfaces;

namespace Projects.Infrastructure.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IEventBusSender _azureServiceBusSender;

    public IntegrationEventService(IEventBusSender azureServiceBusSender)
    {
        this._azureServiceBusSender =
            azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
    }

    private readonly List<IntegrationEvent> _integrationEvents = new();

    public IReadOnlyCollection<IntegrationEvent> IntegrationEvents =>
        _integrationEvents.AsReadOnly();

    public void AddIntegrationEvent(IntegrationEvent integrationEvent)
    {
        _integrationEvents.Add(integrationEvent);
    }

    public void RemoveIntegrationEvent(IntegrationEvent integrationEvent)
    {
        _integrationEvents.Remove(integrationEvent);
    }

    public async Task PublishEventsThroughEventBusAsync()
    {
        foreach (var IntegrationEvent in _integrationEvents)
            await _azureServiceBusSender.PublishMessage(IntegrationEvent);

        _integrationEvents.Clear();
    }
}
