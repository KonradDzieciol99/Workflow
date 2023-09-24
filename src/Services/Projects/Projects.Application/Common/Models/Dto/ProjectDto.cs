namespace Projects.Application.Common.Models.Dto;

public class ProjectDto
{
    public ProjectDto(
        string id,
        string name,
        string iconUrl,
        ICollection<ProjectMemberDto> projectMembers
    )
    {
        Id = id;
        Name = name;
        IconUrl = iconUrl;
        ProjectMembers = projectMembers;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }
    public ICollection<ProjectMemberDto> ProjectMembers { get; set; }
}
