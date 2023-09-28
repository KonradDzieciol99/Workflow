using API.Aggregator.Domain.Commons.Exceptions;
using HttpMessage;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class NotificationService : BaseHttpService, INotificationService
{
    private readonly string _notificationServiceUrl;

    public NotificationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        _notificationServiceUrl =
            configuration.GetValue<string>("urls:internal:notification")
             ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:notification' is missing.");
        ;
    }

    public async Task<bool> Get(string token)
    {
        var sb = new StringBuilder(_notificationServiceUrl);
        sb.Append($"/api/AppNotification/get?Skip={0}&Take={5}");

        return await SendAsync<bool>(new ApiRequest(HttpMethod.Get, sb.ToString(), null));
    }
}
