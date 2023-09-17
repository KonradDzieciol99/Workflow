using HttpMessage;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class NotificationService : BaseHttpService, INotificationService
{
    private readonly string _notificationServiceUrl;
    public NotificationService(HttpClient client, IConfiguration configuration) : base(client)
    {
        _notificationServiceUrl = configuration.GetValue<string>("urls:internal:notification") ?? throw new ArgumentNullException(nameof(configuration)); ;
    }

    public async Task<bool> Get(string token)
    {
        var sb = new StringBuilder(_notificationServiceUrl);
        sb.Append($"/api/AppNotification/get?Skip={0}&Take={5}");

        return await SendAsync<bool>(new ApiRequest(HttpMethod.Get, sb.ToString(), null));
    }
}
