using Projects.Domain.Common.Models;
using Projects.Domain.DomainEvents;

namespace Projects.Domain.AggregatesModel.ProjectAggregate
{
    public class ProjectMember : BaseEntity
    {

        private ProjectMember() { }

        public ProjectMember(string userId, string userEmail, string? photoUrl, ProjectMemberType type/*, string projectId*/)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            UserEmail = userEmail;
            PhotoUrl = photoUrl;
            Type = type;
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string? PhotoUrl { get; set; }
        public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;

        public string ProjectId { get; set; }
        public Project MotherProject { get; set; }
    }
}
