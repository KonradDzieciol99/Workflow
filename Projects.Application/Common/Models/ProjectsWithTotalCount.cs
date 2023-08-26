using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Common.Models;

public class ProjectsWithTotalCount
{
    public ProjectsWithTotalCount(int count, List<ProjectDto> result)
    {
        Count = count;
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public int Count { get; set; }
    public List<ProjectDto> Result { get; set; }
}
