using AutoMapper;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Projects.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.ProjectMembers.DomainEventHandlers;

public class ProjectMemberAddedDomainEventHandler : INotificationHandler<ProjectMemberAddedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public ProjectMemberAddedDomainEventHandler(IIntegrationEventService integrationEventService)
    {
        this._integrationEventService = integrationEventService;
    }
    public Task Handle(ProjectMemberAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ProjectMemberAddedEvent(notification.Member.Id, notification.Member.UserId,
                                    notification.Member.UserEmail, notification.Member.PhotoUrl,
                                     notification.Member.ProjectId,(ProjectMemberTypeMessageBus)notification.Member.Type);

        _integrationEventService.AddIntegrationEvent(@event);

        return Task.CompletedTask;
    }
}
