using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Domain.DomainEvents
{
    public class ProjectMemberAddedDomainEvent : INotification
    {
        public ProjectMemberAddedDomainEvent(ProjectMember member,bool isNewProjectCreator)
        {
            this.Member = member;
            this.IsNewProjectCreator = isNewProjectCreator;
        }
        public ProjectMember Member { get; set; }
        public bool IsNewProjectCreator { get; set; }
    }
}
