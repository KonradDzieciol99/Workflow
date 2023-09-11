namespace API.Aggregator.Services;

public interface INotificationService
{
    Task<bool> Get(string token);
}