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
        var member = this.ProjectMembers.FirstOrDefault(m => m.Id == id);
        if (member is null)
            throw new ProjectDomainException("Such a member does not exist");

        if (member.Type == ProjectMemberType.Leader)
            throw new ProjectDomainException("you cannot remove a member who is a leader");

        this.ProjectMembers.Remove(member);

        this.AddDomainEvent(new ProjectMemberRemovedDomainEvent(member));
    }
    public void RemoveProject()
    {
        this.AddDomainEvent(new ProjectRemovedDomainEvent(this));
    }
    public void UpdateProjectMember(string userId, ProjectMemberType newType)
    {
        var member = this.ProjectMembers.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
            throw new ProjectDomainException("Such a member does not exist");

        if (member.Type == ProjectMemberType.Leader)
            throw new ProjectDomainException("you cannot change a member who is a leader");

        member.Type = newType;

        this.AddDomainEvent(new ProjectMemberUpdatedDomainEvent(member));
    }

    public void Update(string? name, string? iconUrl, string? NewLeaderId)
    {
        if (!string.IsNullOrEmpty(name))
            this.Name = name;

        if (!string.IsNullOrEmpty(iconUrl))
            this.IconUrl = iconUrl;

        if (NewLeaderId is null)
            return;

        var newLeader = this.ProjectMembers.FirstOrDefault(m => m.Id == NewLeaderId);
        var currentLeader = this.ProjectMembers.FirstOrDefault(m => m.Type == ProjectMemberType.Leader);

        if (newLeader is null)
            throw new ProjectDomainException("Alleged new leader is not a member of the team");

        newLeader.Type = ProjectMemberType.Leader;

        if (currentLeader is not null && newLeader.Id == currentLeader.Id)
            throw new ProjectDomainException("the alleged new leader is already the leader of the team");

        if (currentLeader is not null)
            currentLeader.Type = ProjectMemberType.Admin;
    }

    public void AcceptInvitation(string currentUserID)
    {
        var member = ProjectMembers.FirstOrDefault(m => m.UserId == currentUserID);

        if (member is null)
            throw new ProjectDomainException("Such a member does not exist");

        member.AcceptInvitation();

        this.AddDomainEvent(new ProjectMemberAcceptInvitationDomainEvent(member));
    }
    public void DeclineInvitation(string currentUserID)
    {
        var member = ProjectMembers.FirstOrDefault(m => m.UserId == currentUserID);

        if (member is null)
            throw new ProjectDomainException("Such a member does not exist");

        ProjectMembers.Remove(member);

        this.AddDomainEvent(new ProjectMemberDeclineInvitationDomainEvent(member));
    }
}
