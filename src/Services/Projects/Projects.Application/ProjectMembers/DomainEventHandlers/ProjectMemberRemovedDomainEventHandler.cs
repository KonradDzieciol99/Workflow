﻿using MediatR;
using Projects.Application.Common.Interfaces;
using Projects.Application.IntegrationEvents;
using Projects.Domain.DomainEvents;

namespace Projects.Application.ProjectMembers.DomainEventHandlers;

internal class ProjectMemberRemovedDomainEventHandler
    : INotificationHandler<ProjectMemberRemovedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public ProjectMemberRemovedDomainEventHandler(IIntegrationEventService integrationEventService)
    {
        this._integrationEventService =
            integrationEventService
            ?? throw new ArgumentNullException(nameof(integrationEventService));
    }

    public Task Handle(
        ProjectMemberRemovedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var @event = new ProjectMemberRemovedEvent(
            notification.Member.Id,
            notification.Member.ProjectId,
            notification.Member.UserId
        );

        _integrationEventService.AddIntegrationEvent(@event);

        return Task.CompletedTask;
    }
}
