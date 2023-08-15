using Projects.Domain.Common.Enums;
using Projects.Domain.Common.Exceptions;
using Projects.Domain.Common.Models;
using Projects.Domain.DomainEvents;

namespace Projects.Domain.AggregatesModel.ProjectAggregate;

public class ProjectMember : BaseEntity
{
    public ProjectMember(string userId, string userEmail, string? photoUrl, ProjectMemberType type, InvitationStatus invitationStatus)
    {
        Id = Guid.NewGuid().ToString();
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        PhotoUrl = photoUrl;
        Type = type;
        InvitationStatus = invitationStatus;
    }

    private ProjectMember() { }


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

//public ProjectMember(string userId, string userEmail, string? photoUrl, ProjectMemberType type/*, string projectId*/)
//{
//    Id = Guid.NewGuid().ToString();
//    UserId = userId;
//    UserEmail = userEmail;
//    PhotoUrl = photoUrl;
//    Type = type;
//}

//public ProjectMember(string userId, string userEmail, string? photoUrl, ProjectMemberType type/*, string projectId*/)
//{
//    Id = Guid.NewGuid().ToString();
//    UserId = userId;
//    UserEmail = userEmail;
//    PhotoUrl = photoUrl;
//    Type = type;
//}