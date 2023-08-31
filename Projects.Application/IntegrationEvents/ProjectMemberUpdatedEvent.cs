﻿using MessageBus;

namespace Projects.Application.IntegrationEvents;
public class ProjectMemberUpdatedEvent : IntegrationEvent
{
    public ProjectMemberUpdatedEvent(string? photoUrl, string projectMemberId, string userId, string userEmail, int type, int invitationStatus, string projectId)
    {
        PhotoUrl = photoUrl;
        this.ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        Type = type;
        InvitationStatus = invitationStatus;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    }

    public string? PhotoUrl { get; set; }
    public string ProjectMemberId { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public int Type { get; set; }
    public int InvitationStatus { get; set; }
    public string ProjectId { get; set; }
}