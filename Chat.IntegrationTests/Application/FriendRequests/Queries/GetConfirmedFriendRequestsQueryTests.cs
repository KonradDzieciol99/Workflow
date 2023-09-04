using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Commands;
using Chat.Application.FriendRequests.Queries;
using Chat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chat.IntegrationTests.Application.FriendRequests.Queries;
[Collection("Base")]
public class GetConfirmedFriendRequestsQueryTests : IAsyncLifetime 
{
    private readonly Base _base;
    public GetConfirmedFriendRequestsQueryTests(Base @base)
    {
        _base = @base;
    }
    public async Task InitializeAsync()
    {
        await _base._checkpoint.ResetAsync(_base._msSqlContainer.GetConnectionString());
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public static IEnumerable<object[]> GetAppTasksQueryList => new List<object[]>
    {
        new object[]{ new GetConfirmedFriendRequestsQuery(0, 10, "invitedUserEmail@test.com1"), 4 },
        new object[]{ new GetConfirmedFriendRequestsQuery(0, 50, null), 12},
        new object[]{ new GetConfirmedFriendRequestsQuery(0, 50, "cosczegoniema"), 0},
    };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetConfirmedFriendRequestsQuery_ValidData_ReturnsFriendRequestList(GetConfirmedFriendRequestsQuery query,int amount)
    {

        //arrange
        var friendRequests = new List<FriendRequest>()
        {
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId1","invitedUserEmail@test.com1",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId2","invitedUserEmail@test.com2",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId3","invitedUserEmail@test.com3",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId4","invitedUserEmail@test.com4",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId5","invitedUserEmail@test.com5",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId6","invitedUserEmail@test.com6",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId7","invitedUserEmail@test.com7",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId8","invitedUserEmail@test.com8",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId9","invitedUserEmail@test.com9",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId10","invitedUserEmail@test.com10",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId11","invitedUserEmail@test.com11",null),
            new FriendRequest("inviterUserId","inviterUserEmail@test.com",null,"invitedUserId12","invitedUserEmail@test.com12",null),
        };

        foreach (var friendRequest in friendRequests)
        {
            friendRequest.AcceptRequest(friendRequest.InvitedUserId);
        }

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(friendRequests);
        _base._client.SetHeaders(friendRequests[0].InviterUserId, friendRequests[0].InviterUserEmail);

        //act
        var response = await _base._client.GetAsync($"api/FriendRequests/GetConfirmedFriendRequests?{query.ToQueryString()}");

        //assert

        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedFriendRequests = JsonSerializer.Deserialize<List<FriendRequestDto>>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(returnedFriendRequests.Count == amount);
    }
}
