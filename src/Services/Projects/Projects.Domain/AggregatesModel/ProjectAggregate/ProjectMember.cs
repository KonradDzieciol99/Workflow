using Projects.Domain.Common.Enums;
using Projects.Domain.Common.Exceptions;
using Projects.Domain.Common.Models;

namespace Projects.Domain.AggregatesModel.ProjectAggregate;

public class ProjectMember : BaseEntity
{
    private ProjectMember() { }

    public ProjectMember(
        string userId,
        string userEmail,
        string? photoUrl,
        ProjectMemberType type,
        InvitationStatus invitationStatus
    )
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        PhotoUrl = photoUrl;
        Type = type;
        InvitationStatus = invitationStatus;
    }

    public string Id { get; private set; }
    public string UserId { get; private set; }
    public string UserEmail { get; private set; }
    public string? PhotoUrl { get; private set; }
    public ProjectMemberType Type { get; private set; } = ProjectMemberType.Member;
    public InvitationStatus InvitationStatus { get; private set; }
    public string ProjectId { get; private set; }
    public Project MotherProject { get; private set; }

    internal void AcceptInvitation()
    {
        if (InvitationStatus == InvitationStatus.Accepted)
            throw new ProjectDomainException("Member is already confirmed");

        InvitationStatus = InvitationStatus.Accepted;
    }

    public void ChangeType(ProjectMemberType projectMemberType)
    {
        Type = projectMemberType;
    }
}
