using MediatR;
using Projects.Application.Common.Interfaces;
using Projects.Application.IntegrationEvents;
using Projects.Domain.DomainEvents;

namespace Projects.Application.ProjectMembers.DomainEventHandlers;

internal class ProjectMemberDeclineInvitationDomainEventHandler : INotificationHandler<ProjectMemberDeclineInvitationDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public ProjectMemberDeclineInvitationDomainEventHandler(IIntegrationEventService integrationEventService)
    {
        this._integrationEventService = integrationEventService;
    }
    public Task Handle(ProjectMemberDeclineInvitationDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new ProjectMemberDeclineInvitationEvent(notification.Member.Id, notification.Member.UserId,
                                    notification.Member.UserEmail, notification.Member.PhotoUrl, (int)notification.Member.Type,
                                     notification.Member.ProjectId, (int)notification.Member.InvitationStatus,
                                     notification.Member.MotherProject.Name, notification.Member.MotherProject.IconUrl
                                     );

        _integrationEventService.AddIntegrationEvent(@event);

        return Task.CompletedTask;
    }
}
