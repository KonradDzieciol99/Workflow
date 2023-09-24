using MessageBus;

namespace Tasks.Application.IntegrationEvents;

public record TaskDeletedEvent(string Id) : IntegrationEvent;
