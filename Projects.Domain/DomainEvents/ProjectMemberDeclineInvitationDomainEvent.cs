using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Domain.DomainEvents;

public class ProjectMemberDeclineInvitationDomainEvent : INotification
{
    public ProjectMemberDeclineInvitationDomainEvent(ProjectMember member)
    {
        Member = member;
    }
    public ProjectMember Member { get; set; }
}
