using MediatR;
using MessageBus.Events;
using Projects.Application.Common.Interfaces;
using Projects.Domain.DomainEvents;

namespace Projects.Application.Projects.DomainEventHandlers;

public class ProjectRemovedDomainEventHandler : INotificationHandler<ProjectRemovedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public ProjectRemovedDomainEventHandler(IIntegrationEventService integrationEventService)
    {
        this._integrationEventService = integrationEventService;
    }
    public Task Handle(ProjectRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ProjectRemovedEvent(notification.Project.Id, notification.Project.Name, notification.Project.IconUrl);

        _integrationEventService.AddIntegrationEvent(@event);

        return Task.CompletedTask;
    }
}
