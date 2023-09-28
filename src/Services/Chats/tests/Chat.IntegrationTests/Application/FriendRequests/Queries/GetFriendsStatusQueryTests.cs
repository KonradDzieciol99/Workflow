using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Queries;
using Chat.Domain.Entity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestsHelpers.Extensions;

namespace Chat.IntegrationTests.Application.FriendRequests.Queries;

[Collection("Base")]
public class GetFriendsStatusQueryTests : IAsyncLifetime
{
    private readonly Base _base;

    public GetFriendsStatusQueryTests(Base @base)
    {
        _base = @base;
    }

    public async Task InitializeAsync()
    {
        if (_base._checkpoint is null)
            throw new InvalidOperationException("_checkpoint is null and cannot be used.");

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
                new List<string> { "non", "non2", "non2" },
                FriendStatusType.Stranger
            },
            new object[]
            {
                new List<string> { "invitedUserId1", "invitedUserId2", },
                FriendStatusType.InvitedByYou
            },
            new object[]
            {
                new List<string> { "invitedUserId1", "invitedUserId2", },
                FriendStatusType.Friend
            },
        };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetFriendsStatusQuery_ValidData_ReturnsFriendRequestList(
        List<string> ids,
        FriendStatusType friendStatusType
    )
    {
        //arrange
        var friendRequests = new List<FriendRequest>()
        {
            new FriendRequest(
                "inviterUserId",
                "inviterUserEmail@test.com",
                null,
                "invitedUserId1",
                "invitedUserEmail@test.com1",
                null
            ),
            new FriendRequest(
                "inviterUserId",
                "inviterUserEmail@test.com",
                null,
                "invitedUserId2",
                "invitedUserEmail@test.com2",
                null
            ),
        };

        if (friendStatusType == FriendStatusType.Friend)
        {
            foreach (var friendRequest in friendRequests)
            {
                friendRequest.AcceptRequest(friendRequest.InvitedUserId);
            }
        }
        _base._client?.SetHeaders(
            friendRequests[0].InviterUserId,
            friendRequests[0].InviterUserEmail
        );
        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(friendRequests);

        //act
        var response =
            await _base._client.GetAsync(
                $"api/FriendRequests/GetFriendsStatus?usersIds={string.Join("&usersIds=", ids)}"
            )
            ?? throw new InvalidOperationException(
                $"{nameof(IServiceScopeFactory)} Missing required fiield."
            );
        ;

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedFriendStatuses = JsonSerializer.Deserialize<List<FriendStatusDto>>(
            responseString,
            options
        );

        Assert.NotNull(returnedFriendStatuses);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.All(returnedFriendStatuses, item => Assert.Equal(friendStatusType, item.Status));
    }
}
