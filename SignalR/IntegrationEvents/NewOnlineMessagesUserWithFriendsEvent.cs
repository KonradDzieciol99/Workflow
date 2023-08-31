﻿using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;
using StackExchange.Redis;

namespace SignalR.IntegrationEvents;

public class NewOnlineMessagesUserWithFriendsEvent : IntegrationEvent
{
    public IEnumerable<UserDto> NewOnlineUserChatFriends { get; set; }
    public UserDto NewOnlineUser { get; set; }
}
public class NewOnlineMessagesUserWithFriendsEventHandler : IRequestHandler<NewOnlineMessagesUserWithFriendsEvent>
{
    private readonly IHubContext<MessagesHub> _messagesHubContext;
    private readonly IDatabase _redisDb;

    public NewOnlineMessagesUserWithFriendsEventHandler(IConnectionMultiplexer connectionMultiplexer,
        IHubContext<MessagesHub> messagesHubContext)
    {
        _messagesHubContext = messagesHubContext;
        _redisDb = connectionMultiplexer.GetDatabase();
    }
    public async Task Handle(NewOnlineMessagesUserWithFriendsEvent request, CancellationToken cancellationToken)
    {
        List<Task<bool>> listOfOnlineUsers = new();
        foreach (var item in request.NewOnlineUserChatFriends)
        {
            listOfOnlineUsers.Add(_redisDb.KeyExistsAsync($"presence-{item.Email}"));
        }
        var resoult = await Task.WhenAll(listOfOnlineUsers);

        List<UserDto> onlineUsers = new();
        for (int i = 0; i < request.NewOnlineUserChatFriends.Count(); i++)
        {
            if (resoult[i])
            {
                onlineUsers.Add(request.NewOnlineUserChatFriends.ElementAt(i));
            }
        }
        await _messagesHubContext.Clients.User(request.NewOnlineUser.Id).SendAsync("GetOnlineUsers", onlineUsers);
        return;
    }
}
