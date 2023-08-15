using MediatR;
using MessageBus.Events;
using Projects.Application.Common.Interfaces;
using Projects.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.ProjectMembers.DomainEventHandlers;

public class ProjectMemberUpdatedDomainEventHandler : INotificationHandler<ProjectMemberUpdatedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public ProjectMemberUpdatedDomainEventHandler(IIntegrationEventService integrationEventService)
    {
        this._integrationEventService = integrationEventService;
    }
    public Task Handle(ProjectMemberUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ProjectMemberUpdatedEvent(notification.Member.PhotoUrl,notification.Member.Id, notification.Member.UserId,
                                    notification.Member.UserEmail,(int)notification.Member.Type, (int)notification.Member.InvitationStatus,notification.Member.ProjectId);

        _integrationEventService.AddIntegrationEvent(@event);

        return Task.CompletedTask;
    }
}
