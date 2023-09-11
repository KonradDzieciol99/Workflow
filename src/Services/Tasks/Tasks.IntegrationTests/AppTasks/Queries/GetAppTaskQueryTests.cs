using System.Net;
using System.Text.Json;
using Tasks.Application.Common.Models;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using TestsHelpers;

namespace Tasks.IntegrationTests.AppTasks.Queries;

[Collection("Base")]//dzielona klasa wpólna dla wszystkich testów (w obrębie tych które mają ten atrybut z nią) 
public class GetAppTaskQueryTests : IAsyncLifetime //to może zastępowac konstruktor i dispose (bardziej to przejrzyste)
{
    private readonly Base _base;
    public GetAppTaskQueryTests(Base @base)//konstruktor wykonuje się przed każdym testem
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

    [Fact]
    public async Task GetAppTaskQuery_WithValiRouteParams_ReturnsAppTask()
    {

        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
            new AppTask("task2",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),"1"),
            new AppTask("task3",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"1"),
        };

        _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);
        _base._factory.SeedData<Program, ApplicationDbContext, AppTask>(appTasks);

        _base._client.SetHeaders(projectMembers[0].UserId, projectMembers[0].UserEmail);

        //act
        var response = await _base._client.GetAsync($"api/projects/{projectMembers[0].ProjectId}/task/{appTasks[0].Id}");

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedTask = JsonSerializer.Deserialize<AppTaskDto>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(appTasks[0].Id, returnedTask.Id);
        Assert.Equal(appTasks[0].Name, returnedTask.Name);
    }
    [Fact]
    public async Task GetAppTaskQuery_WithInvalidTaskIdRouteParams_ReturnsBadRequest()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };

        var AddedMebers = _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);
        _base._client.SetHeaders(AddedMebers[0].UserId, AddedMebers[0].UserEmail);

        var fakeAppTaskId = "Test!@#";

        //act
        var response = await _base._client.GetAsync($"api/projects/{AddedMebers[0].ProjectId}/task/{fakeAppTaskId}");

        //assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAppTaskQuery_WithInvalidProjectIdRouteParams_ReturnsForbidden()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
            new ProjectMember("2", "userId2", "testUser@email.com2", null, ProjectMemberType.Member,InvitationStatus.Accepted, "1"),
            new ProjectMember("3", "userId3", "testUser@email.com3", null, ProjectMemberType.Member,InvitationStatus.Accepted, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,/*null,null,*/Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
            new AppTask("task2",null,"1",null,/*null,null,*/Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),"2"),
            new AppTask("task3",null,"1",null,/*null,null,*/Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"3"),
        };

        var AddedMebers = _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);
        var addedAppTasks = _base._factory.SeedData<Program, ApplicationDbContext, AppTask>(appTasks);

        _base._client.SetHeaders(AddedMebers[0].UserId, AddedMebers[0].UserEmail);

        var fakeProjectId = "Test123";

        //act
        var response = await _base._client.GetAsync($"api/projects/{fakeProjectId}/task/{addedAppTasks[0].Id}");

        //assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
