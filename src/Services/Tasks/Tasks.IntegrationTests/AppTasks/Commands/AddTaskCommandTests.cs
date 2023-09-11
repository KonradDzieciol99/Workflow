using System.Net;
using System.Text;
using System.Text.Json;
using Tasks.Application.AppTasks.Commands;
using Tasks.Application.Common.Models;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;


namespace Tasks.IntegrationTests.AppTasks.Commands;

[Collection("Base")] //dzielona klasa wpólna dla wszystkich testów (w obrębie tych które mają ten atrybut z nią)
public class AddTaskCommandTests : IAsyncLifetime //to może zastępowac konstruktor i dispose (bardziej to przejrzyste)
{
    private readonly Base _base;
    public AddTaskCommandTests(Base @base)//konstruktor wykonuje się przed każdym testem
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
    public async Task AddTaskCommand_WithValidRouteParamsAndValidAppTask_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };

        _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);

        _base._client.SetHeaders(projectMembers[0].UserId, projectMembers[0].UserEmail);

        var command = new AddTaskCommand("AdedTask", null, projectMembers[0].ProjectId, null, Priority.High, State.Done, new DateTime(2023, 6, 4), new DateTime(2023, 6, 5), "1");

        var content = new StringContent(JsonSerializer.Serialize(command), UTF8Encoding.UTF8, "application/json");
        //act
        var response = await _base._client.PostAsync($"api/projects/{projectMembers[0].ProjectId}/task", content);

        //assert
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedAppTasks = JsonSerializer.Deserialize<AppTaskDto>(responseString, options);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(command.Name, returnedAppTasks.Name);
    }

    [Fact]
    public async Task AddTaskCommand_WithValidRouteParamsAndInvalidAppTask_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader,InvitationStatus.Accepted, "1"),
        };

        _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(projectMembers);

        _base._client.SetHeaders(projectMembers[0].UserId, projectMembers[0].UserEmail);

        var command = new AddTaskCommand(null, null, projectMembers[0].ProjectId, null, (Priority)4, (State)94, new DateTime(2023, 6, 4), new DateTime(2023, 6, 5), "1");
        var content = new StringContent(JsonSerializer.Serialize(command), UTF8Encoding.UTF8, "application/json");

        //act
        var response = await _base._client.PostAsync($"api/projects/{projectMembers[0].ProjectId}/task", content);

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

}
