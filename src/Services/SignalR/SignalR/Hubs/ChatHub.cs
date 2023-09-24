using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.IntegrationEvents;
using StackExchange.Redis;
using System.Security.Claims;

namespace SignalR.Hubs;

[Authorize(Policy = "ApiScope")]
public class ChatHub : Hub
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IEventBusSender _azureServiceBusSender;
    private readonly IDatabase _redisDb;

    public ChatHub(
        IConnectionMultiplexer connectionMultiplexer,
        IEventBusSender azureServiceBusSender
    )
    {
        _connectionMultiplexer = connectionMultiplexer;
        this._azureServiceBusSender = azureServiceBusSender;
        _redisDb = _connectionMultiplexer.GetDatabase();
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext =
            Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
        if (string.IsNullOrEmpty(recipientEmail))
            throw new HubException("User cannot be identified");
        var UserEmail =
            httpContext.User.FindFirstValue(ClaimTypes.Email)
            ?? throw new HubException("User cannot be identified");
        var id =
            httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new HubException("User cannot be identified");
        var picture = httpContext.User.FindFirstValue("picture");

        var groupName = GetGroupName(UserEmail, recipientEmail);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await _redisDb.HashSetAsync(groupName, Context.ConnectionId, UserEmail);

        List<string> groupMembers = new() { UserEmail };

        var values = await _redisDb.HashValuesAsync(groupName);
        if (values.Contains(recipientEmail))
            groupMembers.Add(recipientEmail);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", groupMembers);

        var @event = new UserConnectedToChatEvent(
            new UserDto(id, UserEmail, picture),
            recipientEmail
        );

        await _azureServiceBusSender.PublishMessage(@event);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        var httpContext =
            Context.GetHttpContext() ?? throw new ArgumentNullException(nameof(Context));
        var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
        if (string.IsNullOrEmpty(recipientEmail))
            throw new HubException("User cannot be identified");
        var SenderEmail =
            httpContext.User.FindFirstValue(ClaimTypes.Email)
            ?? throw new HubException("User cannot be identified");

        var groupName = GetGroupName(SenderEmail, recipientEmail);

        await _redisDb.HashDeleteAsync(groupName, Context.ConnectionId);

        List<string> groupMembers = new();

        var values = await _redisDb.HashValuesAsync(groupName);
        if (values.Contains(recipientEmail))
            groupMembers.Add(recipientEmail);
        if (values.Contains(SenderEmail))
            groupMembers.Add(SenderEmail);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", groupMembers);

        await base.OnDisconnectedAsync(ex);
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    public async Task UserIsTyping()
    {
        var httpContext =
            Context.GetHttpContext() ?? throw new ArgumentNullException("httpContext error");
        var recipientEmail = httpContext.Request.Query["RecipientEmail"].ToString();
        if (string.IsNullOrEmpty(recipientEmail))
            throw new HubException("User cannot be identified");
        var userEmail =
            httpContext.User.FindFirstValue(ClaimTypes.Email)
            ?? throw new HubException("User cannot be identified");

        var groupName = GetGroupName(userEmail, recipientEmail);

        await Clients.Group(groupName).SendAsync("UserIsTyping", userEmail);
    }
}
