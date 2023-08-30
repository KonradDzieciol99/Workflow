using MediatR;
using Projects.Application.Common.Interfaces;
using Projects.Application.IntegrationEvents;
using Projects.Domain.DomainEvents;

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
                                    notification.Member.UserEmail, notification.Member.PhotoUrl, (int)notification.Member.Type,
                                     notification.Member.ProjectId, (int)notification.Member.InvitationStatus,
                                     notification.Member.MotherProject.Name, notification.Member.MotherProject.IconUrl,
                                     notification.IsNewProjectCreator
                                     );

        _integrationEventService.AddIntegrationEvent(@event);

        return Task.CompletedTask;
    }
}
