using HttpMessage;
using System.Text;

namespace API.Aggregator.Services;

public class NotificationService : BaseHttpService, INotificationService
{
    private readonly string _notificationServiceUrl;
    public NotificationService(HttpClient client, IConfiguration configuration) : base(client)
    {
        this._notificationServiceUrl = configuration.GetValue<string>("urls:internal:notification") ?? throw new ArgumentNullException(nameof(_notificationServiceUrl)); ;
    }

    public async Task<bool> Get(string token)
    {
        StringBuilder sb = new StringBuilder(_notificationServiceUrl);
        sb.Append($"/api/AppNotification/get?Skip={0}&Take={5}");

        return await this.SendAsync<bool>(new ApiRequest()
        {
            HttpMethod = HttpMethod.Get,
            Url = sb.ToString(),
            AccessToken = token
        });
    }
}
