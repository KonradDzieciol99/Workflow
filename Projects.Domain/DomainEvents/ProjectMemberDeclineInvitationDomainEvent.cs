using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents;

public class ProjectMemberDeclineInvitationDomainEvent : INotification
{
    public ProjectMemberDeclineInvitationDomainEvent(ProjectMember member)
    {
        Member = member;
    }
    public ProjectMember Member { get; set; }
}
