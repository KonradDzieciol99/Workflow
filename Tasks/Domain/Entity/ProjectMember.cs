using Tasks.Domain.Common.Models;

namespace Tasks.Domain.Entity;

public class ProjectMember
{
    private ProjectMember() { }

    public ProjectMember(string id, string userId, string userEmail, string? photoUrl, ProjectMemberType type, InvitationStatus invitationStatus, string projectId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        PhotoUrl = photoUrl;
        Type = type;
        this.invitationStatus = invitationStatus;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    }

    public string Id { get; private set; }
    public string UserId { get; private set; }
    public string UserEmail { get; private set; }
    public string? PhotoUrl { get; private set; }
    public ProjectMemberType Type { get; private set; }
    public InvitationStatus invitationStatus { get; private set; }
    public string ProjectId { get; private set; }
    public ICollection<AppTask> ConductedTasks { get; private set; }
    public ICollection<AppTask> AssignedTasks { get; private set; }
}
//public ProjectMember(string id, string userId, string userEmail, string? photoUrl, ProjectMemberType type, string projectId)
//{
//    Id = id ?? throw new ArgumentNullException(nameof(id));
//    UserId = userId ?? throw new ArgumentNullException(nameof(userId));
//    UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
//    PhotoUrl = photoUrl;
//    Type = type;
//    ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
//}