using System.Threading.Tasks;
using Tasks.Application.AppTasks.Commands;
using Tasks.Domain.Exceptions;
using Tasks.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tasks.Domain.Entity
{
    public class AppTask : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ProjectId { get; set; }
        public string? TaskAssigneeMemberId { get; set; }
        public string? TaskAssigneeMemberEmail { get; set; }
        public string? TaskAssigneeMemberPhotoUrl { get; set; }
        public Priority Priority { get; set; }
        public State State { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }

        public string? TaskLeaderId { get; set; }
        public ProjectMember TaskLeader { get; set; }

        public void UpdateTask(string name, string? description, string? taskAssigneeMemberId, string? taskAssigneeMemberEmail, string? taskAssigneeMemberPhotoUrl, Priority priority, State state, DateTime dueDate, DateTime startDate, string? taskLeaderId)
        {
            if (name == Name
                && description == Description
                && taskAssigneeMemberId == TaskAssigneeMemberId
                && taskAssigneeMemberEmail == TaskAssigneeMemberEmail
                && taskAssigneeMemberPhotoUrl == TaskAssigneeMemberPhotoUrl
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
            TaskAssigneeMemberEmail = taskAssigneeMemberEmail;
            TaskAssigneeMemberPhotoUrl = taskAssigneeMemberPhotoUrl;
            Priority = priority;
            State = state;
            DueDate = dueDate;
            StartDate = startDate;
            TaskLeaderId = taskLeaderId;
            //!!!WAZNE sprawdzić czy wszystkie bedą modified i czy będzie odpowiednie zapytanie do bazy danych
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

    }
}
