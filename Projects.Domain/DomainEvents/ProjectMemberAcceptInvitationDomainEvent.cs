using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public class ProjectMemberAcceptInvitationDomainEvent : INotification
{
    public ProjectMemberAcceptInvitationDomainEvent(ProjectMember member)
    {
        Member = member;
    }
    public ProjectMember Member { get; set; }
}
