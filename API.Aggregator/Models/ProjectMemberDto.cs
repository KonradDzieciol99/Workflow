namespace API.Aggregator.Models;

public class ProjectMemberDto
{
    public ProjectMemberDto()
    {

    }
    public ProjectMemberDto(string id, string userId, string userEmail, ProjectMemberType type, InvitationStatus invitationStatus, string projectId)
    {
        Id = id;
        UserId = userId;
        UserEmail = userEmail;
        Type = type;
        ProjectId = projectId;
        InvitationStatus = invitationStatus;
    }

    public string Id { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public ProjectMemberType Type { get; set; }
    public InvitationStatus InvitationStatus { get; set; }
    public string? PhotoUrl { get; set; }
    public string ProjectId { get; set; }
}

public enum InvitationStatus
{
    Invited,
    Accepted
}
