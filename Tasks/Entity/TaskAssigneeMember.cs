namespace Tasks.Entity
{
    public class TaskAssigneeMember:BaseEntity
    {

        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string PhotoUrl { get; set; }

        //public string AppTaskId { get; set; }
        //public AppTask MotherAppTask { get; set; }
    }
}
