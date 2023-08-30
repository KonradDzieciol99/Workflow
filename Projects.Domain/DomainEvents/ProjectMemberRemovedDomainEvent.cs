using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public class ProjectMemberRemovedDomainEvent : INotification
{
    public ProjectMemberRemovedDomainEvent(ProjectMember member)
    {
        Member = member;
    }
    public ProjectMember Member { get; set; }
}
