using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.IntegrationEvents;
using StackExchange.Redis;
using System.Security.Claims;

namespace SignalR.Hubs;

[Authorize(Policy = "ApiScope")]
public class PresenceHub : Hub
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IEventBusSender _messageBus;
    private readonly IDatabase _redisDb;

    public PresenceHub(IConnectionMultiplexer connectionMultiplexer, IEventBusSender eventBusSender)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _messageBus = eventBusSender ?? throw new ArgumentNullException(nameof(eventBusSender));
        _redisDb = _connectionMultiplexer.GetDatabase();
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext =
            Context.GetHttpContext() ?? throw new HubException("HttpContext is null");
        var email =
            httpContext.User.FindFirstValue(ClaimTypes.Email)
                ?? throw new HubException("User cannot be identified");
        var id =
            httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new HubException("User cannot be identified");

        await _redisDb.SetAddAsync($"presence-{email}", Context.ConnectionId);

        var newOnlineUserEvent = new UserOnlineEvent(new UserDto(id, email, null));
        await _messageBus.PublishMessage(newOnlineUserEvent);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext =
            Context.GetHttpContext() ?? throw new HubException("HttpContext is null");
        var email =
            httpContext.User.FindFirstValue(ClaimTypes.Email)
                ?? throw new HubException("User cannot be identified");
        var id =
            httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new HubException("User cannot be identified");

        await _redisDb.SetRemoveAsync($"presence-{email}", Context.ConnectionId);
        var ConnectionLength = await _redisDb.SetLengthAsync($"presence-{email}");

        if (ConnectionLength == 0)
        {
            await _redisDb.KeyDeleteAsync($"presence-{email}");

            var newOnlineUserEvent = new UserOfflineEvent(new UserDto(id, email, null));
            await _messageBus.PublishMessage(newOnlineUserEvent);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
