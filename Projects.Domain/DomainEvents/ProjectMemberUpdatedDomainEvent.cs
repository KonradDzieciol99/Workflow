using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public record ProjectMemberUpdatedDomainEvent(ProjectMember Member) : INotification;
