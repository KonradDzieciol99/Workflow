using Azure;
using Chat.Application.Common.Models;
using Chat.Application.FriendRequests.Commands;
using Chat.Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestsHelpers.Extensions;

namespace Chat.IntegrationTests.Application.FriendRequests.Commands;

[Collection("Base")]
public class CreateFriendRequestCommandTests : IAsyncLifetime
{
    private readonly Base _base;

    public CreateFriendRequestCommandTests(Base @base)
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
    public async Task CreateFriendRequestCommand_ValidData_ReturnsCreated()
    {
        //arrange
        _base._client.SetHeaders("testId", "testEmail@test.com");
        CreateFriendRequestCommand command =
            new("testTargetUserId", "testTargetUserEmail@test.com", null);

        //act
        var response = await _base._client.PostAsync(
            $"api/FriendRequests",
            command.ToStringContent()
        );

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedAppTasks = JsonSerializer.Deserialize<FriendRequestDto>(
            responseString,
            options
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(returnedAppTasks);
        Assert.False(returnedAppTasks.Confirmed);
    }
}
