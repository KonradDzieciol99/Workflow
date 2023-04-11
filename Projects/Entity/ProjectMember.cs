using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Projects.Entity
{
    public class ProjectMember : BaseEntity
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

        public ProjectMember(string userId, string userEmail, Project motherProject, ProjectMemberType type = ProjectMemberType.Member)
        {
            UserId = userId;
            UserEmail = userEmail;
            Type = type;
            MotherProject = motherProject;
        }

        //public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string PhotoUrl { get; set; }
        public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;

        public string ProjectId { get; set; }
        public Project MotherProject { get; set; }

    }
}
