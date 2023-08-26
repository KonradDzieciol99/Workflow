using Tasks.Domain.Common.Exceptions;
using Tasks.Domain.Common.Models;

namespace Tasks.Domain.Entity;

public class AppTask : BaseEntity
{
    private AppTask() { }
    public AppTask(string name, string? description, string projectId, string? taskAssigneeMemberId,/* string? taskAssigneeMemberEmail, string? taskAssigneeMemberPhotoUrl,*/ Priority priority, State state, DateTime dueDate, DateTime startDate, string? taskLeaderId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        TaskAssigneeMemberId = taskAssigneeMemberId;
        //TaskAssigneeMemberEmail = taskAssigneeMemberEmail;
        //TaskAssigneeMemberPhotoUrl = taskAssigneeMemberPhotoUrl;
        Priority = priority;
        State = state;
        DueDate = dueDate;
        StartDate = startDate;
        TaskLeaderId = taskLeaderId;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string ProjectId { get; private set; }
    //public string? TaskAssigneeMemberId { get; private set; }
    //public string? TaskAssigneeMemberEmail { get; private set; }
    //public string? TaskAssigneeMemberPhotoUrl { get; private set; }
    public string? TaskAssigneeMemberId { get; private set; }
    public ProjectMember TaskAssignee { get; private set; }

    public Priority Priority { get; private set; }
    public State State { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime StartDate { get; private set; }

    public string? TaskLeaderId { get; private set; }
    public ProjectMember TaskLeader { get; private set; }

    public void UpdateTask(string? name, string? description, string? taskAssigneeMemberId/*, string? taskAssigneeMemberEmail, string? taskAssigneeMemberPhotoUrl*/, Priority? priority, State? state, DateTime? dueDate, DateTime? startDate, string? taskLeaderId)
    {
        if (name == Name
            && description == Description
            && taskAssigneeMemberId == TaskAssigneeMemberId
            //&& taskAssigneeMemberEmail == TaskAssigneeMemberEmail
            //&& taskAssigneeMemberPhotoUrl == TaskAssigneeMemberPhotoUrl
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
        //TaskAssigneeMemberEmail = taskAssigneeMemberEmail;
        //TaskAssigneeMemberPhotoUrl = taskAssigneeMemberPhotoUrl;
        Priority = priority ?? Priority;
        State = state ?? State;
        DueDate = dueDate ?? DueDate;
        StartDate = startDate ?? StartDate;
        TaskLeaderId = taskLeaderId;
        //!!!WAZNE sprawdzić czy wszystkie bedą modified i czy będzie odpowiednie zapytanie do bazy danych
    }

}


//public void UpdateTask(UpdateAppTaskCommand request)
//{
//    return  command.Name == Name
//        && command.Description == Description
//        && command.TaskAssigneeMemberId == TaskAssigneeMemberId
//        && command.TaskAssigneeMemberEmail == TaskAssigneeMemberEmail
//        && command.TaskAssigneeMemberPhotoUrl ==TaskAssigneeMemberPhotoUrl
//        && command.Priority == Priority
//        && command.State == State
//        && command.DueDate == DueDate
//        && command.StartDate == StartDate;
//}

//public ICollection<string>  Comments { get; set; }
//public ICollection<string>  Hisotry { get; set; } //logs of actions
// public ICollection<TaskAssigneeMember> TaskAssigneeMembers { get; set; } ///?????