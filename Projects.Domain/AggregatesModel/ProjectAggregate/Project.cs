using MediatR;
using Projects.Domain.Common.Models;
using Projects.Domain.DomainEvents;
using Projects.Domain.Exceptions;

namespace Projects.Domain.AggregatesModel.ProjectAggregate
{
    public class Project : BaseEntity
    {
        private Project(){ }
        public Project(string name, string iconUrl)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            IconUrl = iconUrl;
            ProjectMembers = new List<ProjectMember>();
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string IconUrl { get; private set; }
        public ICollection<ProjectMember> ProjectMembers { get; private set; }

        public void AddProjectMember(ProjectMember newMember)
        {
            ProjectMembers.Add(newMember);

            this.AddDomainEvent(new ProjectMemberAddedDomainEvent(newMember));
        }
        public void RemoveProjectMember(string userId)
        {
            var member = this.ProjectMembers.FirstOrDefault(m => m.UserId == userId);
            if (member is null)
                throw new ProjectDomainException("Such a member does not exist");

            if (member.Type == ProjectMemberType.Leader)
                throw new ProjectDomainException("you cannot remove a member who is a leader");

            this.ProjectMembers.Remove(member);

            this.AddDomainEvent(new ProjectMemberRemovedDomainEvent(member));
        }
        public void RemoveProject()
        {
            this.AddDomainEvent(new ProjectRemovedDomainEvent(this));
        }
        public void UpdateProjectMember(string userId,ProjectMemberType newType)
        {
            var member = this.ProjectMembers.FirstOrDefault(m => m.UserId == userId);
            if (member is null)
                throw new ProjectDomainException("Such a member does not exist");

            if (member.Type == ProjectMemberType.Leader)
                throw new ProjectDomainException("you cannot change a member who is a leader");

            member.Type = newType;

            this.AddDomainEvent(new ProjectMemberUpdatedDomainEvent(member));
        }
    }
}
