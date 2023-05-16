namespace Projects.Application.Common.Models.Dto
{
    public class CreateProjectMemberDto
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string PhotoUrl { get; set; }
        public ProjectMemberType Type { get; set; }
        public string ProjectId { get; set; }
    }
}
