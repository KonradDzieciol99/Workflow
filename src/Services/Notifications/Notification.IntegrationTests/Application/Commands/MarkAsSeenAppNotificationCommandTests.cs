using Notification.Application.AppNotifications.Commands;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.IntegrationTests.Application.Commands;

[Collection("Base")]
public class MarkAsSeenAppNotificationCommandTests : IAsyncLifetime
{
    private readonly Base _base;
    public MarkAsSeenAppNotificationCommandTests(Base @base)
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
    public async Task MarkAsSeenAppNotificationCommand_WithValidData_ReturnsNoContent()
    {
        //arrange
        var appNotifications = new List<AppNotification>()
        {
            new AppNotification("1",
                                NotificationType.WelcomeNotification,
                                DateTime.UtcNow,
                                "test",
                                null,
                                null,
                                null,
                                false),
        };
        _base._factory.SeedData<Program, ApplicationDbContext, AppNotification>(appNotifications);
        _base._client.SetHeaders("1", "1");
        var command = new MarkAsSeenAppNotificationCommand(appNotifications[0].Id);

        //act
        var response = await _base._client.PutAsync($"api/AppNotification/{command.Id}",null);

        //assert
        var result = await _base._factory.FindAsync<Program, ApplicationDbContext, AppNotification>(appNotifications[0].Id);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(result.Displayed);
    }
}
