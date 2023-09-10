using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public record ProjectMemberRemovedDomainEvent(ProjectMember Member) : INotification;
