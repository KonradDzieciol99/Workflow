using MediatR;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Domain.DomainEvents
{
    public class ProjectMemberAddedDomainEvent : INotification
    {
        public ProjectMemberAddedDomainEvent(ProjectMember member, bool isNewProjectCreator)
        {
            this.Member = member;
            this.IsNewProjectCreator = isNewProjectCreator;
        }
        public ProjectMember Member { get; set; }
        public bool IsNewProjectCreator { get; set; }
    }
}
