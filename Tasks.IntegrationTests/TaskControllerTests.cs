using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tasks.Application.Common.Models;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Infrastructure.DataAccess;
using Tasks.Infrastructure.Repositories;

namespace Tasks.IntegrationTests;

[Collection("Base")]
public class TaskControllerTests
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public TaskControllerTests(Base @base)
    {
        _factory = @base._factory;
        _client = @base._client;
    }

    [Fact]
    public async Task Get_WithValidQuery_ReturnsAppTask()
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

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            _dbContext.ProjectMembers.AddRange(projectMembers);
            _dbContext.AppTasks.AddRange(appTasks);
            var result = _dbContext.SaveChanges();


            //var unitofwork = scope.ServiceProvider.GetService<IUnitOfWork>();
            //var result2 = await unitofwork.ProjectMemberRepository.CheckIfUserIsAMemberOfProject(appTasks[0].Id, "1");

            //var ApplicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            ////var result3 = ApplicationDbContext.ProjectMembers.Any(x => x.UserId == "1" && x.ProjectId == appTasks[0].Id);
            //var result3 = _dbContext.ProjectMembers.Any(x => x.UserId == "1");

        }
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, projectMembers[0].UserId);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserEmail, projectMembers[0].UserEmail);

        //act

        var response = await _client.GetAsync($"api/projects/1/task/{appTasks[0].Id}");
        //assert
        var responseString = await response.Content.ReadAsStringAsync();


        var options = new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true };
        var returnedTask = JsonSerializer.Deserialize<AppTaskDto>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(appTasks[0].Id, returnedTask.Id);
        Assert.Equal(appTasks[0].StartDate, returnedTask.StartDate);
        Assert.Equal(appTasks[0].Name, returnedTask.Name);

    }




}
