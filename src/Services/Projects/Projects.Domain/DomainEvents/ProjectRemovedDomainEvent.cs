using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public record ProjectRemovedDomainEvent(Project Project) : INotification;
