using System.Net;
using System.Text;
using System.Text.Json;
using Tasks.Application.AppTasks.Commands;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;

namespace Tasks.IntegrationTests.AppTasks.Commands;

[Collection("Base")]//dzielona klasa wpólna dla wszystkich testów (w obrębie tych które mają ten atrybut z nią)
public class DeleteAppTaskCommandTests : IAsyncLifetime //to może zastępowac konstruktor i dispose (bardziej to przejrzyste)
{
    private readonly Base _base;
    public DeleteAppTaskCommandTests(Base @base)//konstruktor wykonuje się przed każdym testem
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
    public async Task DeleteAppTaskCommand_ValidData_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
        };

        var addedMebers = _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);
        var addedAppTasks = _base._factory.SeedData<Program, ApplicationDbContext, AppTask>(appTasks);

        _base._client.SetHeaders(addedMebers[0].UserId, addedMebers[0].UserEmail);

        var command = new DeleteAppTaskCommand(appTasks[0].Id, appTasks[0].ProjectId);

        //act
        var response = await _base._client.DeleteAsync($"api/projects/{command.ProjectId}/task/{command.Id}");

        //assert

        var item = await _base._factory.FindAsync<Program, ApplicationDbContext, AppTask>(addedAppTasks[0].Id);

        Assert.Null(item);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    [Fact]
    public async Task DeleteAppTaskCommand_InValidData_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
        };

        var addedMebers = _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);
        var addedAppTasks = _base._factory.SeedData<Program, ApplicationDbContext, AppTask>(appTasks);

        _base._client.SetHeaders(addedMebers[0].UserId, addedMebers[0].UserEmail);

        var command = new DeleteAppTaskCommand(appTasks[0].Id, appTasks[0].ProjectId);
        var fakeTaskId = "444344";
        //act
        var response = await _base._client.DeleteAsync($"api/projects/{command.ProjectId}/task/{fakeTaskId}");

        //assert

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
