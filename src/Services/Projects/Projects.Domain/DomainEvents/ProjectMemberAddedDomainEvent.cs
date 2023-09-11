using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public record ProjectMemberAddedDomainEvent(ProjectMember Member, bool IsNewProjectCreator) : INotification;
