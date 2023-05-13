using Projects.Domain.Common.Models;
using Projects.Domain.DomainEvents;

namespace Projects.Domain.Entities
{
    public class ProjectMember : BaseEntity
    {

        private ProjectMember(){}

        public ProjectMember(string userId, string userEmail, string photoUrl, ProjectMemberType type, string projectId)
        {
            UserId = userId;
            UserEmail = userEmail;
            PhotoUrl = photoUrl;
            Type = type;
            ProjectId = projectId;

            this.AddProjectMember(this);
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string PhotoUrl { get; set; }
        public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;

        public string ProjectId { get; set; }
        public Project MotherProject { get; set; }

        private void AddProjectMember(ProjectMember projectMember)
        {
            var projectMemberAddedDomainEvent = new ProjectMemberAddedDomainEvent(projectMember);

            this.AddDomainEvent(projectMemberAddedDomainEvent);
        }

        public bool CanBeDeleted()
        {
            return Type != ProjectMemberType.Leader;
        }
        public async Task ChangeType()
        {

        }

    }
}
