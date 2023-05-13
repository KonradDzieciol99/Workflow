using MediatR;
using Projects.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Domain.DomainEvents
{
    public class ProjectMemberAddedDomainEvent : INotification
    {
        public ProjectMemberAddedDomainEvent(ProjectMember member)
        {
            Member = member;
        }
        public ProjectMember Member { get; set; }

    }
}
