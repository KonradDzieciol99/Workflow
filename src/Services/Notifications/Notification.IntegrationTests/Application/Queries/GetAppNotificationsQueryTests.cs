using Notification.Application.AppNotifications.Queries;
using Notification.Domain.Common.Enums;
using Notification.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Notification.IntegrationTests.Application.Queries;
[Collection("Base")]
public class GetAppNotificationsQueryTests : IAsyncLifetime
{
    private readonly Base _base;
    public GetAppNotificationsQueryTests(Base @base)
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

    public static IEnumerable<object[]> GetAppTasksQueryList => new List<object[]>
    {
        new object[]{ new GetAppNotificationsQuery(0, 50), 50},
        new object[]{ new GetAppNotificationsQuery(0, 12), 12},
        new object[]{ new GetAppNotificationsQuery(30, 20), 20},
    };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetAppNotificationsQueryTests_WithValidData_ReturnsNotificationsList(GetAppNotificationsQuery query, int amount)
    {
        //arrange

        var appNotifications = Base.GetFakeAppNotification("1", false, 50);

        _base._factory.SeedData<Program, ApplicationDbContext, AppNotification>(appNotifications);
        _base._client.SetHeaders("1", "1");

        //act
        var response = await _base._client.GetAsync($"api/AppNotification?{query.ToQueryString()}");

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<List<AppNotification>>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(amount, result.Count);
    }
}