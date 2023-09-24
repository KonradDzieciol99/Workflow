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
using TestsHelpers.Extensions;
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
        new object[]{ new GetConfirmedFriendRequestsQuery(0, 50, "invitedUserEmail@test.com1"), 1},
        new object[]{ new GetConfirmedFriendRequestsQuery(0, 12, null), 12},
        new object[]{ new GetConfirmedFriendRequestsQuery(0, 50, "cosczegoniema"), 0},
    };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetConfirmedFriendRequestsQuery_ValidData_ReturnsFriendRequestList(GetConfirmedFriendRequestsQuery query,int amount)
    {

        //arrange
        var friendRequests = Base.GetFakeFriendRequests(50, true, "inviterUserId", "inviterUserEmail", null,null);

        var request = new FriendRequest(friendRequests[0].InviterUserId, friendRequests[0].InviterUserEmail, null, "invitedUserId@test.com1", "invitedUserEmail@test.com1", null);
        request.AcceptRequest(request.InvitedUserId);

        friendRequests.Add(request);

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(friendRequests);
        _base._client.SetHeaders(friendRequests[0].InviterUserId, friendRequests[0].InviterUserEmail);

        //act
        var response = await _base._client.GetAsync($"api/FriendRequests/GetConfirmedFriendRequests?{query.ToQueryString()}");

        //assert

        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedFriendRequests = JsonSerializer.Deserialize<List<FriendRequestDto>>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(returnedFriendRequests.Count, amount);
    }
}
