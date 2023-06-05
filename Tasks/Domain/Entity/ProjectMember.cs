using Tasks.Domain.Common.Models;

namespace Tasks.Domain.Entity;

public class ProjectMember
{
    private ProjectMember() { }

    public ProjectMember(string id, string userId, string userEmail, string? photoUrl, ProjectMemberType type, string projectId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        PhotoUrl = photoUrl;
        Type = type;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    }

    public string Id { get; private set; }
    public string UserId { get; private set; }
    public string UserEmail { get; private set; }
    public string? PhotoUrl { get; private set; }
    public ProjectMemberType Type { get; private set; } = ProjectMemberType.Member;
    public string ProjectId { get; private set; }
    public ICollection<AppTask> ConductedTasks { get; private set; }
}
