using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Security.Claims;

namespace SignalR.Hubs;

[Authorize(Policy = "ApiScope")]
public class MessagesHub : Hub
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    //private readonly IMapper _mapper;
    private readonly IDatabase _redisDb;
    public MessagesHub(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        //_mapper = mapper;
        _redisDb = _connectionMultiplexer.GetDatabase();
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var httpContext = Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        var SenderEmail = httpContext.User.FindFirstValue(ClaimTypes.Email) ?? throw new HubException("User cannot be identified");


        await base.OnDisconnectedAsync(exception);
    }
}
