using Projects.Entity;

namespace Projects.Models.Dto
{
    public class ProjectDto
    {
        public ProjectDto(string id, string name, ICollection<ProjectMemberDto> projectMembers)
        {
            Id = id;
            Name = name;
            ProjectMembers = projectMembers;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProjectMemberDto> ProjectMembers { get; set; }
    }
}
