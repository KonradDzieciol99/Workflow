namespace Tasks.Entity
{
    public class ProjectMember
    {
        public ProjectMember()
        {

        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string PhotoUrl { get; set; }
        public ProjectMemberType Type { get; set; } = ProjectMemberType.Member;
        public string ProjectId { get; set; }
    }
}
