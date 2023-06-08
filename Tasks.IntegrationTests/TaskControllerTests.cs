using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tasks.Application.AppTasks.Commands;
using Tasks.Application.AppTasks.Queries;
using Tasks.Application.Common.Models;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.DataAccess;
using Tasks.Infrastructure.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tasks.IntegrationTests;

[Collection("Base")]
public class TaskControllerTests
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly WebApplicationFactory<Program> _factory;

    public TaskControllerTests(Base @base)//konstruktor wykonuje się przed każdym testem
    {
        _factory = @base._factory;
        _client = @base._client;
        _configuration = @base._configuration;
        @base._checkpoint.ResetAsync(_configuration.GetConnectionString("TestDb")!);
    }
    private void SetHeaders(string userId, string userEmail)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, userId);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserEmail, userEmail);
    }
    private (List<ProjectMember>? projectMembers, List<AppTask>? appTasks) Seed(List<ProjectMember>? projectMembers, List<AppTask>? appTasks)
    {
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>() ?? throw new ArgumentNullException(nameof(ApplicationDbContext));

           if (projectMembers is not null)
                _dbContext.ProjectMembers.AddRange(projectMembers);

            if (appTasks is not null)
                _dbContext.AppTasks.AddRange(appTasks);

            var result = _dbContext.SaveChanges();
        }

        return (projectMembers, appTasks);
    }
    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    [Fact]
    public async Task Get_WithValiRouteParams_ReturnsAppTask()
    {

        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
            new AppTask("task2",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),"1"),
            new AppTask("task3",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"1"),
        };
        var result = Seed(projectMembers, appTasks);

        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        //act

        var response = await _client.GetAsync($"api/projects/{result.projectMembers[0].ProjectId}/task/{result.appTasks[0].Id}");
        //assert
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true };
        var returnedTask = JsonSerializer.Deserialize<AppTaskDto>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(result.appTasks[0].Id, returnedTask.Id);
        Assert.Equal(result.appTasks[0].StartDate, returnedTask.StartDate);
        Assert.Equal(result.appTasks[0].Name, returnedTask.Name);
    }

    [Fact]
    public async Task Get_WithInvalidTaskIdRouteParams_ReturnsBadRequest()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
            new AppTask("task2",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),"1"),
            new AppTask("task3",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"1"),
        };
        var result = Seed(projectMembers, appTasks);

        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        var fakeAppTaskId = "Test!@#";
        //act

        var response = await _client.GetAsync($"api/projects/{result.projectMembers[0].ProjectId}/task/{fakeAppTaskId}");

        //assert

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithInvalidProjectIdRouteParams_ReturnsForbidden()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
            new ProjectMember("2", "userId2", "testUser@email.com2", null, ProjectMemberType.Member, "1"),
            new ProjectMember("3", "userId3", "testUser@email.com3", null, ProjectMemberType.Member, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
            new AppTask("task2",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),"2"),
            new AppTask("task3",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"3"),
        };
        var result = Seed(projectMembers, appTasks);
        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        var fakeProjectId = "Test123";
        //act

        var response = await _client.GetAsync($"api/projects/{fakeProjectId}/task/{result.appTasks[0].Id}");

        //assert

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithInvalidRouteParamsAndInvaliQueryParams_ReturnsForbidden()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
        };
        var result = Seed(projectMembers, appTasks);

        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        var fakeProjectId = "Test1234565678";

        var fakeQueryParams = $"ProjectId={fakeProjectId}&Take=10";
        //act

        var response = await _client.GetAsync($"api/projects/{fakeProjectId}/task?{fakeQueryParams}");

        //assert

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("ProjectId=1&Take=10")]
    [InlineData("ProjectId=1&Take=10&Search=task1")]//reszta w tescie integracyjnym validatory i dokładne rzeczy z bazy danych
    public async Task Get_WithValidRouteParamsAndQueryParams_ReturnsAppTaskList(string queryParams)
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),"1"),
            new AppTask("task2",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),"1"),
            new AppTask("task3",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"1"),
            new AppTask("task4",null,"2",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),"1"),
            new AppTask("task4",null,"5",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,7),new DateTime(2023,6,8),null),
        };
        var result = Seed(projectMembers, appTasks);

        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        //act
        var response = await _client.GetAsync($"api/projects/{result.projectMembers[0].ProjectId}/task?{queryParams}");
        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedAppTasks = JsonSerializer.Deserialize<AppTaskDtosWithTotalCount>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithValidRouteParamsAndValidAppTask_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };

        var result = Seed(projectMembers, null);
        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        var command = new AddTaskCommand("AdedTask", null, result.projectMembers[0].ProjectId, null, null, null, Priority.High, State.Done, new DateTime(2023, 6, 4), new DateTime(2023, 6, 5));
        var content = new StringContent(JsonSerializer.Serialize(command), UTF8Encoding.UTF8, "application/json");
        //act

        var response = await _client.PostAsync($"api/projects/{result.projectMembers[0].ProjectId}/task", content);

        //assert
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedAppTasks = JsonSerializer.Deserialize<AppTaskDto>(responseString, options);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(command.Name, returnedAppTasks.Name);
    }
    [Fact]
    public async Task Post_WithValidRouteParamsAndInvalidAppTask_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };

        var result = Seed(projectMembers, null);
        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        var command = new AddTaskCommand(null, null, result.projectMembers[0].ProjectId, null, null, null, (Priority)4, (State)94, new DateTime(2023, 6, 4), new DateTime(2023, 6, 5));
        var content = new StringContent(JsonSerializer.Serialize(command), UTF8Encoding.UTF8, "application/json");

        //act

        var response = await _client.PostAsync($"api/projects/{result.projectMembers[0].ProjectId}/task", content);

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_WithValidRouteParamsAndValidUptatedModel_ReturnsAppTask()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),projectMembers[0].Id),
            new AppTask("task2",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,5),new DateTime(2023,6,7),projectMembers[0].Id),
        };
        var result = Seed(projectMembers, appTasks);
        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        var updatedDescription = "updated description";
        var command = new UpdateAppTaskCommand(result.appTasks[0].Id, result.appTasks[0].Name, updatedDescription, result.appTasks[0].ProjectId, result.appTasks[0].TaskAssigneeMemberId, result.appTasks[0].TaskAssigneeMemberEmail, result.appTasks[0].TaskAssigneeMemberPhotoUrl, result.appTasks[0].Priority, result.appTasks[0].State, result.appTasks[0].DueDate, result.appTasks[0].StartDate, result.appTasks[0].TaskLeaderId);

        var content = new StringContent(JsonSerializer.Serialize(command), UTF8Encoding.UTF8, "application/json");

        //act

        var response = await _client.PutAsync($"api/projects/{result.projectMembers[0].ProjectId}/task/{result.appTasks[0].Id}", content);

        //assert
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedAppTasks = JsonSerializer.Deserialize<AppTaskDto>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(updatedDescription, returnedAppTasks.Description);
    }
    [Fact]
    public async Task Delete_WithValidRouteParams_ReturnsNoContent()
    {
        //arrange
        var projectMembers = new List<ProjectMember>()
        {
            new ProjectMember("1", "userId1", "testUser@email.com1", null, ProjectMemberType.Leader, "1"),
        };
        var appTasks = new List<AppTask>()
        {
            new AppTask("task1",null,"1",null,null,null,Priority.Low,State.ToDo,new DateTime(2023,6,4),new DateTime(2023,6,5),projectMembers[0].Id),
        };

        var result = Seed(projectMembers, appTasks);
        SetHeaders(result.projectMembers[0].UserId, result.projectMembers[0].UserEmail);

        var updatedDescription = "updated description";
        var command = new UpdateAppTaskCommand(result.appTasks[0].Id, result.appTasks[0].Name, updatedDescription, result.appTasks[0].ProjectId, result.appTasks[0].TaskAssigneeMemberId, result.appTasks[0].TaskAssigneeMemberEmail, result.appTasks[0].TaskAssigneeMemberPhotoUrl, result.appTasks[0].Priority, result.appTasks[0].State, result.appTasks[0].DueDate, result.appTasks[0].StartDate, result.appTasks[0].TaskLeaderId);

        var content = new StringContent(JsonSerializer.Serialize(command), UTF8Encoding.UTF8, "application/json");

        //act

        var response = await _client.DeleteAsync($"api/projects/{result.projectMembers[0].ProjectId}/task/{result.appTasks[0].Id}");

        //assert
        var item = await FindAsync<AppTask>(result.appTasks[0].Id);
        Assert.Equal(null, item);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
