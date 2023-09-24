using MediatR;
using Projects.Application.Common.Interfaces;
using Projects.Application.IntegrationEvents;
using Projects.Domain.DomainEvents;

namespace Projects.Application.ProjectMembers.DomainEventHandlers;

public class ProjectMemberAcceptInvitationDomainEventHandler
    : INotificationHandler<ProjectMemberAcceptInvitationDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public ProjectMemberAcceptInvitationDomainEventHandler(
        IIntegrationEventService integrationEventService
    )
    {
        this._integrationEventService =
            integrationEventService
            ?? throw new ArgumentNullException(nameof(integrationEventService));
    }

    public Task Handle(
        ProjectMemberAcceptInvitationDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var @event = new ProjectMemberAcceptInvitationEvent(
            notification.Member.Id,
            notification.Member.UserId,
            notification.Member.UserEmail,
            notification.Member.PhotoUrl,
            (int)notification.Member.Type,
            notification.Member.ProjectId,
            (int)notification.Member.InvitationStatus,
            notification.Member.MotherProject.Name,
            notification.Member.MotherProject.IconUrl
        );

        _integrationEventService.AddIntegrationEvent(@event);

        return Task.CompletedTask;
    }
}
