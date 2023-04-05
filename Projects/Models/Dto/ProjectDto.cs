using Projects.Entity;

namespace Projects.Models.Dto
{
    public class ProjectDto
    {
        public ProjectDto(string id, string name, string iconUrl, ICollection<ProjectMemberDto> projectMembers, ProjectMemberDto leader)
        {
            Id = id;
            Name = name;
            IconUrl = iconUrl;
            ProjectMembers = projectMembers;
            Leader = leader;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public ICollection<ProjectMemberDto> ProjectMembers { get; set; }
        public ProjectMemberDto Leader { get; set; }
    }
}
