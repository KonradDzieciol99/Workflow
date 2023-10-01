using API.Aggregator.Application.Common.Models;
using API.Aggregator.Application.FriendRequestsAggregate.Queries;
using HttpMessage;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestsHelpers.Extensions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace API.Aggreggator.IntegrationTests.Application.FriendRequestsAggregate.Queries;

[Collection("Base")]
public class SearchFriendAggregateQueryTests : IAsyncLifetime
{
    private readonly Base _base;

    public SearchFriendAggregateQueryTests(Base @base)
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

    [Fact]
    public async Task SearchFriendAggregateQuery_ValidData_ReturnsSerchedUserStatuses()
    {
        //arrange
        var usersList = new List<UserDto>()
        {
            new UserDto("1", "jan.kowalski@example.com", null),
            new UserDto("2", "jan.nowak@example.com", null)
        };

        var searchedUserDto = new List<SearchedUserDto>()
        {
            new SearchedUserDto(
                usersList[0].Id,
                usersList[0].Email,
                usersList[0].PhotoUrl,
                FriendStatusType.Friend
            ),
            new SearchedUserDto(
                usersList[1].Id,
                usersList[1].Email,
                usersList[1].PhotoUrl,
                FriendStatusType.Stranger
            ),
        };

        _base._mockServer
            .Given(
                requestMatcher: Request
                    .Create()
                    .WithPath("/api/IdentityUser/search/*")
                    .WithParam("take")
                    .WithParam("skip")
                    .UsingGet()
            )
            .RespondWith(
                provider: Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(JsonSerializer.Serialize(usersList))
            );

        _base._mockServer
            .Given(
                requestMatcher: Request
                    .Create()
                    .WithPath("/api/FriendRequests/GetFriendsStatus")
                    .WithParam("usersIds")
                    .UsingGet()
            )
            .RespondWith(
                provider: Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(JsonSerializer.Serialize(searchedUserDto))
            );

        var query = new SearchFriendAggregateQuery("jan", 10, 0);

        _base._client.SetHeaders("0", "executingUser@email");

        //act
        var response = await _base._client.GetAsync(
            $"api/FriendRequests/search/{query.Email}?{query.ToQueryString()}"
        );

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var searchedUsers = JsonSerializer.Deserialize<List<SearchedUserDto>>(
            responseString,
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(searchedUsers);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(usersList.Count, searchedUsers.Count);
    }
}
