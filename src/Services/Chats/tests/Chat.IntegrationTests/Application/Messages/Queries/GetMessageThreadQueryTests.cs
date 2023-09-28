using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Queries;
using Chat.Application.Messages.Commands;
using Chat.Application.Messages.Queries;
using Chat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestsHelpers.Extensions;

namespace Chat.IntegrationTests.Application.Messages.Queries;

[Collection("Base")]
public class GetMessageThreadQueryTests : IAsyncLifetime
{
    private readonly Base _base;

    public GetMessageThreadQueryTests(Base @base)
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
                new GetMessageThreadQuery("invitedUserEmail@test.com1", "invitedUserId1", 0, 10)
                //3
            },
        };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetMessageThreadQuery_ValidData_ReturnsMessages(
        GetMessageThreadQuery query
        //int amount
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
        };
        var messages = new List<Message>()
        {
            new(
                friendRequests[0].InviterUserId,
                friendRequests[0].InviterUserEmail,
                friendRequests[0].InvitedUserId,
                friendRequests[0].InvitedUserEmail,
                "test"
            ),
            new(
                friendRequests[0].InviterUserId,
                friendRequests[0].InviterUserEmail,
                friendRequests[0].InvitedUserId,
                friendRequests[0].InvitedUserEmail,
                "test2"
            ),
            new(
                friendRequests[0].InvitedUserId,
                friendRequests[0].InvitedUserEmail,
                friendRequests[0].InviterUserId,
                friendRequests[0].InviterUserEmail,
                "test3"
            ),
        };

        foreach (var friendRequest in friendRequests)
        {
            friendRequest.AcceptRequest(friendRequest.InvitedUserId);
        }

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(friendRequests);
        _base._factory.SeedData<Program, ApplicationDbContext, Message>(messages);
        _base._client.SetHeaders(
            friendRequests[0].InviterUserId,
            friendRequests[0].InviterUserEmail
        );

        //act
        var response = await _base._client.GetAsync($"api/Messages?{query.ToQueryString()}");

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedMessageDtos = JsonSerializer.Deserialize<List<MessageDto>>(
            responseString,
            options
        );

        Assert.NotNull(returnedMessageDtos);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(messages.Count, returnedMessageDtos.Count);
    }
}
