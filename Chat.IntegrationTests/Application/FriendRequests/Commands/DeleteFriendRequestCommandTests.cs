using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Commands;
using Chat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestsHelpers;

namespace Chat.IntegrationTests.Application.FriendRequests.Commands;
[Collection("Base")]
public class DeleteFriendRequestCommandTests : IAsyncLifetime
{
    private readonly Base _base;
    public DeleteFriendRequestCommandTests(Base @base)
    {
        _base = @base;
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    public async Task InitializeAsync()
    {
        await _base._checkpoint.ResetAsync(_base._msSqlContainer.GetConnectionString());
    }
    [Fact]
    public async Task DeleteFriendRequestCommand_ValidData_ReturnsNoContent()
    {
        //arrange
        var friendRequests = new List<FriendRequest>()
        {
            new FriendRequest("inviterUserId","inviterUserEmail@@test.com",null,"invitedUserId","invitedUserEmail@test.com",null),
        };

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(friendRequests);
        _base._client.SetHeaders(friendRequests[0].InviterUserId, friendRequests[0].InviterUserEmail);
        DeleteFriendRequestCommand command = new(friendRequests[0].InvitedUserId);

        //act
        var response = await _base._client.DeleteAsync($"api/FriendRequests/{command.TargetUserId}");

        //assert
        var friendReques = await _base._factory.FindAsync<Program, ApplicationDbContext, FriendRequest>(friendRequests[0].InviterUserId, friendRequests[0].InvitedUserId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Null(friendReques);
    }
}
