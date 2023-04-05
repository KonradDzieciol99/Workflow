namespace Projects.Entity
{
    public class Project
    {
        public Project()
        {
            
        }

        public Project(string name, string iconUrl, string leaderId, ProjectMember leader, ICollection<ProjectMember> projectMembers)
        {
            Name = name;
            IconUrl = iconUrl;
            LeaderId = leaderId;
            Leader = leader;
            ProjectMembers = projectMembers;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }

        public string LeaderId { get; set; }
        public ProjectMember Leader { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }

    }
}
