namespace Projects.Entity
{
    public class Project
    {
        public Project()
        {
            
        }
        public Project(string name, ICollection<ProjectMember> projectMembers)
        {
            Name = name;
            ProjectMembers = projectMembers;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }

    }
}
