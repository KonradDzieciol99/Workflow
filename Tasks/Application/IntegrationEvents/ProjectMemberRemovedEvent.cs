using MessageBus;

namespace Tasks.Application.IntegrationEvents;

public class ProjectMemberRemovedEvent : IntegrationEvent
{
    public ProjectMemberRemovedEvent(string projectMemberId, string projectId, string userId)
    {
        ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    public string ProjectMemberId { get; set; }
    public string ProjectId { get; set; }
    public string UserId { get; set; }
}