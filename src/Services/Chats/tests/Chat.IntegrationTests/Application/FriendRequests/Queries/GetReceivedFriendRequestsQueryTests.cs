using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Queries;
using Chat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chat.IntegrationTests.Application.FriendRequests.Queries;
[Collection("Base")]
public class GetReceivedFriendRequestsQueryTests: IAsyncLifetime
{
    private readonly Base _base;
    public GetReceivedFriendRequestsQueryTests(Base @base)
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
        new object[]{ new GetReceivedFriendRequestsQuery(0, 10, "inviterUserEmail@test.com1"), 4 },
        new object[]{ new GetReceivedFriendRequestsQuery(0, 50, null), 12},
        new object[]{ new GetReceivedFriendRequestsQuery(0, 50, "cosczegoniema"), 0},
    };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetReceivedFriendRequestsQuery_ValidData_ReturnsFriendRequestList(GetReceivedFriendRequestsQuery query, int amount)
    {

        //arrange
        var friendRequests = new List<FriendRequest>()
        {
            new FriendRequest("inviterUserId1","inviterUserEmail@test.com1",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId2","inviterUserEmail@test.com2",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId3","inviterUserEmail@test.com3",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId4","inviterUserEmail@test.com4",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId5","inviterUserEmail@test.com5",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId6","inviterUserEmail@test.com6",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId7","inviterUserEmail@test.com7",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId8","inviterUserEmail@test.com8",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId9","inviterUserEmail@test.com9",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId10","inviterUserEmail@test.com10",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId11","inviterUserEmail@test.com11",null,"invitedUserId","invitedUserEmail@test.com",null),
            new FriendRequest("inviterUserId12","inviterUserEmail@test.com12",null,"invitedUserId","invitedUserEmail@test.com",null),
        };

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(friendRequests);
        _base._client.SetHeaders(friendRequests[0].InvitedUserId, friendRequests[0].InvitedUserEmail);

        //act
        var response = await _base._client.GetAsync($"api/FriendRequests/GetReceivedFriendRequests?{query.ToQueryString()}");

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedFriendRequests = JsonSerializer.Deserialize<List<FriendRequestDto>>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(returnedFriendRequests.Count == amount);
    }
}
