using Projects.Domain.Common.Enums;
using Projects.Domain.Common.Exceptions;
using Projects.Domain.Common.Models;

namespace Projects.Domain.AggregatesModel.ProjectAggregate;

public class ProjectMember : BaseEntity
{
#pragma warning disable CS8618
    private ProjectMember() { }
#pragma warning restore CS8618
    public ProjectMember(string userId, string userEmail, string? photoUrl, ProjectMemberType type, InvitationStatus invitationStatus)
    {
        //Id = Guid.NewGuid().ToString();
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        PhotoUrl = photoUrl;
        Type = type;
        InvitationStatus = invitationStatus;
    }

    public string Id { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string? PhotoUrl { get; set; }
    public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;
    public InvitationStatus InvitationStatus { get; set; }
    public string ProjectId { get; set; }
    public Project MotherProject { get; set; }

    internal void AcceptInvitation()
    {
        if (InvitationStatus == InvitationStatus.Accepted)
            throw new ProjectDomainException("Member is already confirmed");

        InvitationStatus = InvitationStatus.Accepted;
    }
}