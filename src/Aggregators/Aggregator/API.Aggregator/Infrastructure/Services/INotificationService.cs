namespace API.Aggregator.Infrastructure.Services;

public interface INotificationService
{
    Task<bool> Get(string token);
}