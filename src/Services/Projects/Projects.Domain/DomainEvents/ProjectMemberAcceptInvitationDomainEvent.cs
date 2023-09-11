using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public record ProjectMemberAcceptInvitationDomainEvent(ProjectMember Member) : INotification;
