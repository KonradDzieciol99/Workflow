using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using System.Runtime.CompilerServices;

namespace Projects.Domain.DomainEvents;

public class ProjectMemberDeclineInvitationDomainEvent : INotification
{
    public ProjectMemberDeclineInvitationDomainEvent(ProjectMember member,Project project)
    {
        Member = member;
        Project = project;
    }
    public ProjectMember Member { get; }
    public Project Project { get; }
}
