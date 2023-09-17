namespace Projects.Application.Common.Models;

public class MemberStatusDto
{
    public required string UserId { get; set; }
    public required MemberStatusType Status { get; set; }
}
