using MediatR;
using Projects.Domain.Common.Models;

namespace Projects.Domain.AggregatesModel.ProjectAggregate
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

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string IconUrl { get; private set; }
        public ICollection<ProjectMember> ProjectMembers { get; private set; }

        public void AddProjectMember(ProjectMember newMember)
        {
            ProjectMembers.Add(newMember);
        }
    }
}
