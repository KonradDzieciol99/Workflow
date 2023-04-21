namespace API.Aggregator.Services
{
    public interface IIdentityServerService
    {
        Task<T> CheckIfUserExistsAsync<T>(string email, string token);
    }
}