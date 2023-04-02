using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Projects.Entity
{
    public class ProjectMember
    {
        public ProjectMember()
        {
            
        }
        public ProjectMember(string userId, string userEmail, ProjectMemberType type = ProjectMemberType.Member)
        {
            UserId = userId;
            UserEmail = userEmail;
            Type = type;
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;

        public string ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
