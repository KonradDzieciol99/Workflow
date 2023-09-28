using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Queries;
using Chat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestsHelpers.Extensions;

namespace Chat.IntegrationTests.Application.FriendRequests.Queries;

[Collection("Base")]
public class GetReceivedFriendRequestsQueryTests : IAsyncLifetime
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

    public static IEnumerable<object[]> GetAppTasksQueryList =>
        new List<object[]>
        {
            new object[]
            {
                new GetReceivedFriendRequestsQuery(0, 10, "inviterUserEmail@test.com1"),
                1
            },
            new object[] { new GetReceivedFriendRequestsQuery(0, 12, null), 12 },
            new object[] { new GetReceivedFriendRequestsQuery(0, 50, "cosczegoniema"), 0 },
        };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetReceivedFriendRequestsQuery_ValidData_ReturnsFriendRequestList(
        GetReceivedFriendRequestsQuery query,
        int amount
    )
    {
        //arrange
        var friendRequests = Base.GetFakeFriendRequests(
            50,
            false,
            null,
            null,
            "invitedUserId",
            "invitedUserEmail"
        );
        var request = new FriendRequest(
            "inviterUserId",
            "inviterUserEmail@test.com1",
            null,
            friendRequests[0].InvitedUserId,
            friendRequests[0].InvitedUserEmail,
            null
        );
        friendRequests.Add(request);

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(friendRequests);
        _base._client.SetHeaders(
            friendRequests[0].InvitedUserId,
            friendRequests[0].InvitedUserEmail
        );

        //act
        var response = await _base._client.GetAsync(
            $"api/FriendRequests/GetReceivedFriendRequests?{query.ToQueryString()}"
        );

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedFriendRequests = JsonSerializer.Deserialize<List<FriendRequestDto>>(
            responseString,
            options
        );

        Assert.NotNull(returnedFriendRequests);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(returnedFriendRequests.Count == amount);
    }
}
