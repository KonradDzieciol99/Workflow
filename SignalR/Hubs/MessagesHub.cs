using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace SignalR.Hubs;

[Authorize(Policy = "ApiScope")]
public class MessagesHub : Hub
{
    public MessagesHub(IConnectionMultiplexer connectionMultiplexer)
    {
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
