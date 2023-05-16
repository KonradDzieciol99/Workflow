using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Common.Models
{
    public class ProjectsWithTotalCount
    {
        public int Count { get; set; }
        public List<ProjectDto> Result { get; set; }
    }
}
