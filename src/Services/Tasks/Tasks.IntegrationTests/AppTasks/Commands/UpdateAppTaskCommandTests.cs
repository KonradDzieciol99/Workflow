using System.Net;
using System.Text;
using System.Text.Json;
using Tasks.Application.AppTasks.Commands;
using Tasks.Application.Common.Models;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using TestsHelpers.Extensions;

namespace Tasks.IntegrationTests.AppTasks.Commands;

[Collection("Base")] //dzielona klasa wpólna dla wszystkich testów (w obrębie tych które mają ten atrybut z nią)
public class UpdateAppTaskCommandTests : IAsyncLifetime //to może zastępowac konstruktor i dispose (bardziej to przejrzyste)
{
    private readonly Base _base;

    public UpdateAppTaskCommandTests(Base @base) //konstruktor wykonuje się przed każdym testem
    {
        _base = @base;
    }

    public async Task InitializeAsync() //wykonuje się przed każdym testem
    {
        await _base._checkpoint.ResetAsync(_base._msSqlContainer.GetConnectionString());
    }

    public Task DisposeAsync() //wykonuje się po każdym tescie
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task UpdateAppTaskCommandTests_WithValidRouteParamsAndValidUptatedModel_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember(
                "1",
                "userId1",
                "testUser@email.com1",
                null,
                ProjectMemberType.Leader,
                InvitationStatus.Accepted,
                "1"
            ),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask(
                "task1",
                null,
                "1",
                null,
                Priority.Low,
                State.ToDo,
                new DateTime(2023, 6, 4),
                new DateTime(2023, 6, 5),
                projectMembers[0].Id
            ),
            new AppTask(
                "task2",
                null,
                "1",
                null,
                Priority.Low,
                State.ToDo,
                new DateTime(2023, 6, 5),
                new DateTime(2023, 6, 7),
                projectMembers[0].Id
            ),
        };

        var AddedMebers = _base._factory.SeedData<Program, ApplicationDbContext, ProjectMember>(
            projectMembers
        );
        var addedAppTasks = _base._factory.SeedData<Program, ApplicationDbContext, AppTask>(
            appTasks
        );

        _base._client.SetHeaders(AddedMebers[0].UserId, AddedMebers[0].UserEmail);

        var updatedDescription = "updated description";
        var command = new UpdateAppTaskCommand(
            addedAppTasks[0].Id,
            addedAppTasks[0].Name,
            updatedDescription,
            addedAppTasks[0].ProjectId,
            addedAppTasks[0].TaskAssigneeMemberId,
            addedAppTasks[0].Priority,
            addedAppTasks[0].State,
            addedAppTasks[0].DueDate,
            addedAppTasks[0].StartDate,
            addedAppTasks[0].TaskLeaderId
        );

        var content = new StringContent(
            JsonSerializer.Serialize(command),
            UTF8Encoding.UTF8,
            "application/json"
        );

        //act
        var response = await _base._client.PutAsync(
            $"api/projects/{AddedMebers[0].ProjectId}/task/{addedAppTasks[0].Id}",
            content
        );

        //assert
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedAppTasks = JsonSerializer.Deserialize<AppTaskDto>(responseString, options);

        Assert.NotNull(returnedAppTasks);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(updatedDescription, returnedAppTasks.Description);
    }
}
