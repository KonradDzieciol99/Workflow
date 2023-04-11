using Projects.Entity;
using Projects.Models.Dto;

namespace Projects.Models
{
    public class ProjectsWithTotalCount
    {
        public int Count { get; set; }
        public List<ProjectDto> Result { get; set; }
    }
}
