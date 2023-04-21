namespace API.Aggregator.Services
{
    public interface IProjectsService
    {
        Task<bool> CheckIfUserIsAMemberOfProject(string userId, string projectId, string token);
    }
}
