namespace Projects.Entity
{
    public class Project : BaseEntity
    {
        public Project()
        {
            
        }

        public Project(string name, string iconUrl)
        {
            Name = name;
            IconUrl = iconUrl;
        }

        //public string Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }
    }
}
