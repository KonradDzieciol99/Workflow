using Chat.Application.FriendRequests.Commands;
using Chat.Application.Messages.Commands;
using Chat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestsHelpers.Extensions;

namespace Chat.IntegrationTests.Application.Messages.Commands;

[Collection("Base")]
public class SendMessageCommandTests : IAsyncLifetime
{
    private readonly Base _base;

    public SendMessageCommandTests(Base @base)
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
    public async Task SendMessageCommand_ValidData_ReturnsCreated()
    {
        //arrange
        var FriendRequests = new List<FriendRequest>()
        {
            new(
                "inviterUserId",
                "inviterUserEmail",
                null,
                "invitedUserId",
                "invitedUserEmail",
                null
            ),
        };

        FriendRequests[0].AcceptRequest("invitedUserId");

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(FriendRequests);

        _base._client.SetHeaders("inviterUserId", "inviterUserEmail");

        SendMessageCommand command = new("invitedUserId", "invitedUserEmail", "test message");

        //act
        var response = await _base._client.PostAsync($"api/Messages", command.ToStringContent());

        //assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
