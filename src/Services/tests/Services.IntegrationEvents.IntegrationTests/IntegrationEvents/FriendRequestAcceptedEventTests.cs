using Chat.Application.FriendRequests.Commands;
using Chat.Domain.Entity;
using Chat.Infrastructure.DataAccess;
using Notification.Application.AppNotifications.Queries;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using System.Text.Json;

namespace Services.IntegrationEvents.IntegrationTests.IntegrationEvents;
[Collection("Base")]
public class FriendRequestAcceptedEventTests : IAsyncLifetime
{
    private readonly Base _base;
    public FriendRequestAcceptedEventTests(Base @base)
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
    public async Task AcceptFriendRequestCommand_InValidData_NoExistingFriendRequests_ReturnsForbidden()
    {
        //arrange
        var FriendRequests = ChatBase.GetFakeFriendRequests();
        _base._chatFactory.SeedData<Chat.Program, ApplicationDbContext, FriendRequest>(FriendRequests);
        _base._chatClient.SetHeaders(FriendRequests[0].InvitedUserId, FriendRequests[0].InvitedUserEmail);
        var command = new AcceptFriendRequestCommand(FriendRequests[0].InviterUserId);

        _base._notificationClient.SetHeaders(FriendRequests[0].InvitedUserId, FriendRequests[0].InvitedUserEmail);
        var query = new GetAppNotificationsQuery(0, 1);

        //act
        await _base._chatClient.PutAsync($"api/FriendRequests/{command.TargetUserId}", null);

        //assert

        var counter = 0;

        List<AppNotification>? result = new();

        while (counter < 15)
        {
            var response = await _base._notificationClient.GetAsync($"api/AppNotification?{query.ToQueryString()}");
            var responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            result = JsonSerializer.Deserialize<List<AppNotification>>(responseString, options);

            if (result.Count > 0)
                break;


            counter++;
            await Task.Delay(100);
        }

        Assert.Equal(1,result?.Count);
        Assert.True(result[0].NotificationType == NotificationType.FriendRequestAccepted);

    }
}