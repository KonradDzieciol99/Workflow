using MessageBus;

namespace Projects.Application.IntegrationEvents;

public record ProjectRemovedEvent(string ProjectId, string Name, string IconUrl) : IntegrationEvent;
