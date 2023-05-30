using Tasks.Models;

namespace Tasks.Domain.Entity;

public class ProjectMember
{
    private ProjectMember() { }

    public ProjectMember(string id, string userId, string userEmail, string photoUrl, ProjectMemberType type, string projectId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        PhotoUrl = photoUrl ?? throw new ArgumentNullException(nameof(photoUrl));
        Type = type;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    }

    public string Id { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string PhotoUrl { get; set; }
    public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;
    public string ProjectId { get; set; }
    public ICollection<AppTask> ConductedTasks { get; set; }
}
