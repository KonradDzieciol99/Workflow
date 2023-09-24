using Tasks.Domain.Common.Models;

namespace Tasks.Application.Common.Models;

public class AppTaskDto
{
    public AppTaskDto(
        string id,
        string name,
        string? description,
        string projectId,
        string? taskAssigneeMemberId,
        ProjectMemberDto? taskAssignee,
        Priority priority,
        State state,
        DateTime dueDate,
        DateTime startDate,
        string? taskLeaderId,
        ProjectMemberDto? taskLeader
    )
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        TaskAssigneeMemberId = taskAssigneeMemberId;
        TaskAssignee = taskAssignee;
        Priority = priority;
        State = state;
        DueDate = dueDate;
        StartDate = startDate;
        TaskLeaderId = taskLeaderId;
        TaskLeader = taskLeader;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string ProjectId { get; set; }
    public string? TaskAssigneeMemberId { get; set; }
    public ProjectMemberDto? TaskAssignee { get; set; }
    public Priority Priority { get; set; }
    public State State { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime StartDate { get; set; }
    public string? TaskLeaderId { get; set; }
    public ProjectMemberDto? TaskLeader { get; set; }
}
