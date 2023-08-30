using System.Net;
using System.Text.Json;
using System.Web;
using Tasks.Application.AppTasks.Queries;
using Tasks.Application.Common.Models;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using TestsHelpers;

namespace Tasks.IntegrationTests.AppTasks.Queries;

[Collection("Base")]//dzielona klasa wpólna dla wszystkich testów (w obrębie tych które mają ten atrybut z nią)
public class GetAppTasksQueryTests : IAsyncLifetime //to może zastępowac konstruktor i dispose (bardziej to przejrzyste)
{
    private readonly Base _base;
    public GetAppTasksQueryTests(Base @base)//konstruktor wykonuje się przed każdym testem
    {
        _base = @base;
    }
    public async Task InitializeAsync()//wykonuje się przed każdym testem
    {
        await _base._checkpoint.ResetAsync(_base._msSqlContainer.GetConnectionString());
    }
    public Task DisposeAsync()//wykonuje się po każdym tescie
    {
        return Task.CompletedTask;
    }

    public static IEnumerable<object[]> GetAppTasksQueryList => new List<object[]>
    {
        new object[]{ new GetAppTasksQuery("1", 0, 10, null, null, null, null, null, null) },
        new object[]{ new GetAppTasksQuery("1", 0, 10, null, null, null, null, "task3", null) },
    };

    [Theory]
    [MemberData(nameof(GetAppTasksQueryList))]
    public async Task GetAppTasksQuery_WithValidRouteParamsAndQueryParams_ReturnsAppTaskList(GetAppTasksQuery query)
    {

        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
            new AppTask("task2",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),"1"),
            new AppTask("task3",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"1"),
            new AppTask("task4",null,"2",null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"1"),
            new AppTask("task4",null,"5",null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),null),
        };

        _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);
        _base._factory.SeedData<Program, ApplicationDbContext, AppTask>(appTasks);

        _base._client.SetHeaders(projectMembers[0].UserId, projectMembers[0].UserEmail);

        //act
        var queryParams = query.ToQueryString();
        var response = await _base._client.GetAsync($"api/projects/{projectMembers[0].ProjectId}/task?{queryParams}");

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedAppTasks = JsonSerializer.Deserialize<AppTaskDtosWithTotalCount>(responseString, options);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAppTasksQuery_WithInvalidRouteParamsAndInvaliQueryParams_ReturnsForbidden()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };

        var AddedMebers = _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);

        _base._client.SetHeaders(AddedMebers[0].UserId, AddedMebers[0].UserEmail);

        var fakeQuery = new GetAppTasksQuery("2", 0, 10, null, null, null, null, null, null);
        //act
        var response = await _base._client.GetAsync($"api/projects/{fakeQuery.ProjectId}/task?{fakeQuery.ToQueryString()}");

        //assert

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
