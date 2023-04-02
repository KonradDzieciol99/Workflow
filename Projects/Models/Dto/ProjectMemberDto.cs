using Projects.Entity;

namespace Projects.Models.Dto
{
    public class ProjectMemberDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;
        public string ProjectId { get; set; }

    }
}
