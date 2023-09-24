using Projects.Domain.Common.Enums;
using Projects.Domain.Common.Exceptions;
using Projects.Domain.Common.Models;
using Projects.Domain.DomainEvents;

namespace Projects.Domain.AggregatesModel.ProjectAggregate;

public class Project : BaseEntity
{
    private Project() { }

    public Project(string name, string iconUrl, ProjectMember creator)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        IconUrl = iconUrl;
        ProjectMembers = new List<ProjectMember>() { creator };

        this.AddDomainEvent(new ProjectMemberAddedDomainEvent(creator, true));
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
    public string IconUrl { get; private set; }
    public ICollection<ProjectMember> ProjectMembers { get; private set; }

    public void AddProjectMember(ProjectMember newMember)
    {
        ProjectMembers.Add(newMember);

        this.AddDomainEvent(new ProjectMemberAddedDomainEvent(newMember, false));
    }

    public void RemoveProjectMember(string id)
    {
        var member =
            this.ProjectMembers.FirstOrDefault(m => m.Id == id)
            ?? throw new ProjectDomainException("Such a member does not exist");

        if (member.Type == ProjectMemberType.Leader)
            throw new ProjectDomainException("you cannot remove a member who is a leader");

        this.ProjectMembers.Remove(member);

        this.AddDomainEvent(new ProjectMemberRemovedDomainEvent(member));
    }

    public void RemoveProject()
    {
        this.AddDomainEvent(new ProjectRemovedDomainEvent(this));
    }

    public void UpdateProjectMember(
        string performingActionUserId,
        string targetUserId,
        ProjectMemberType targetUserNewType
    )
    {
        var targetMember =
            this.ProjectMembers.FirstOrDefault(m => m.UserId == targetUserId)
            ?? throw new ProjectDomainException("Such a member does not exist.");
        var performingActionMember =
            this.ProjectMembers.FirstOrDefault(m => m.UserId == performingActionUserId)
            ?? throw new ProjectDomainException(
                "User performing the operation is not part of the project."
            );

        if (targetMember.Type == ProjectMemberType.Leader)
            throw new ProjectDomainException("you cannot change a member who is a leader");

        if (
            performingActionMember.Type != ProjectMemberType.Leader
            && targetUserNewType == ProjectMemberType.Leader
        )
            throw new ProjectDomainException(
                "Only the project leader can transfer the title of leader."
            );

        if (targetUserNewType == ProjectMemberType.Leader)
            performingActionMember.ChangeType(ProjectMemberType.Admin);

        targetMember.ChangeType(targetUserNewType);

        this.AddDomainEvent(new ProjectMemberUpdatedDomainEvent(targetMember));
    }

    public void Update(string name, string iconUrl, string NewLeaderId)
    {
        bool wasChanged = false;

        if (name != this.Name)
        {
            wasChanged = true;
            this.Name = name;
        }
        if (iconUrl != this.IconUrl)
        {
            wasChanged = true;
            this.IconUrl = iconUrl;
        }

        var newLeader = this.ProjectMembers.FirstOrDefault(m => m.Id == NewLeaderId);
        var currentLeader = this.ProjectMembers.FirstOrDefault(
            m => m.Type == ProjectMemberType.Leader
        );

        if (newLeader is null)
            throw new ProjectDomainException("Alleged new leader is not a member of the team");

        if (newLeader.Id != currentLeader.Id)
        {
            newLeader.ChangeType(ProjectMemberType.Leader);
            currentLeader.ChangeType(ProjectMemberType.Admin);
            wasChanged = true;
        }

        if (!wasChanged)
            throw new ProjectDomainException("No changes were made");
    }

    public void AcceptInvitation(string currentUserID)
    {
        var member =
            ProjectMembers.FirstOrDefault(m => m.UserId == currentUserID)
            ?? throw new ProjectDomainException("Such a member does not exist");

        member.AcceptInvitation();

        this.AddDomainEvent(new ProjectMemberAcceptInvitationDomainEvent(member));
    }

    public void DeclineInvitation(string currentUserID)
    {
        var member =
            ProjectMembers.FirstOrDefault(m => m.UserId == currentUserID)
            ?? throw new ProjectDomainException("Such a member does not exist");

        ProjectMembers.Remove(member);

        this.AddDomainEvent(new ProjectMemberDeclineInvitationDomainEvent(member, this));
    }
}
