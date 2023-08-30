namespace Projects.Application.Common.Models;

public class CheckIfUserIsAMemberOfProjectRequest
{
    public string ProjectId { get; set; }
    public string UserId { get; set; }
}
