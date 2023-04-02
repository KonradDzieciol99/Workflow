using Projects.Entity;

namespace Projects.Models.Dto
{
    public class ProjectDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProjectMemberDto> ProjectMembers { get; set; }
    }
}
