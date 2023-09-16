using Bogus;
using Chat.Application.FriendRequests.Commands;
using Chat.Domain.Entity;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.IntegrationTests.Application.FriendRequests.Commands;
[Collection("Base")]
public class AcceptFriendRequestCommandTests : IAsyncLifetime
{
    private readonly Base _base;
    public AcceptFriendRequestCommandTests(Base @base)
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
    public async Task AcceptFriendRequestCommand_ValidData_ReturnsNoContent()
    {
        //arrange
        var FriendRequests = Base.GetFakeFriendRequests();

        _base._factory.SeedData<Program, ApplicationDbContext, FriendRequest>(FriendRequests);

        _base._client.SetHeaders(FriendRequests[0].InvitedUserId, FriendRequests[0].InvitedUserEmail);

        var command = new AcceptFriendRequestCommand(FriendRequests[0].InviterUserId);

        //act
        var response = await _base._client.PutAsync($"api/FriendRequests/{command.TargetUserId}", null);

        //assert
        var friendReques = await _base._factory.FindAsync<Program, ApplicationDbContext, FriendRequest>(FriendRequests[0].InviterUserId, FriendRequests[0].InvitedUserId);

        Assert.NotNull(friendReques);
        Assert.Equal(true, friendReques.Confirmed);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    [Fact]
    public async Task AcceptFriendRequestCommand_InValidData_NoExistingFriendRequests_ReturnsForbidden()
    {
        //arrange
        _base._client.SetHeaders("userId", "userEmail");

        var command = new AcceptFriendRequestCommand("targetUserId");

        //act
        var response = await _base._client.PutAsync($"api/FriendRequests/{command.TargetUserId}", null);

        //assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }



}
