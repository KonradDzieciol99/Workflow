using Tasks.Models;

namespace Tasks.Entity
{
    public class AppTask : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }
        public string? TaskAssigneeMemberId { get; set; }
        public string? TaskAssigneeMemberEmail { get; set; }
        public string? TaskAssigneeMemberPhotoUrl { get; set; }
        public Priority Priority { get; set; }
        public State State { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        //public ICollection<string>  Comments { get; set; }
        //public ICollection<string>  Hisotry { get; set; } //logs of actions
        // public ICollection<TaskAssigneeMember> TaskAssigneeMembers { get; set; } ///?????

    }
}
