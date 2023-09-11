using Tasks.Domain.Common.Exceptions;
using Tasks.Domain.Common.Models;

namespace Tasks.Domain.Entity;

public class AppTask : BaseEntity
{
    private AppTask() { }
    public AppTask(string name, string? description, string projectId, string? taskAssigneeMemberId, Priority priority, State state, DateTime dueDate, DateTime startDate, string? taskLeaderId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        TaskAssigneeMemberId = taskAssigneeMemberId;
        Priority = priority;
        State = state;
        DueDate = dueDate;
        StartDate = startDate;
        TaskLeaderId = taskLeaderId;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string ProjectId { get; private set; }
    public string? TaskAssigneeMemberId { get; private set; }
    public ProjectMember TaskAssignee { get; private set; }
    public Priority Priority { get; private set; }
    public State State { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public string? TaskLeaderId { get; private set; }
    public ProjectMember TaskLeader { get; private set; }

    public void UpdateTask(string name, string? description, string? taskAssigneeMemberId, Priority? priority, State? state, DateTime? dueDate, DateTime? startDate, string? taskLeaderId)
    {
        if (name == Name
            && description == Description
            && taskAssigneeMemberId == TaskAssigneeMemberId
            && priority == Priority
            && state == State
            && dueDate == DueDate
            && startDate == StartDate
            && taskLeaderId == TaskLeaderId)
        {
            throw new TaskDomainException("No changes have been made.");
        }

        Name = name;
        Description = description;
        TaskAssigneeMemberId = taskAssigneeMemberId;
        Priority = priority ?? Priority;
        State = state ?? State;
        DueDate = dueDate ?? DueDate;
        StartDate = startDate ?? StartDate;
        TaskLeaderId = taskLeaderId;
    }
}