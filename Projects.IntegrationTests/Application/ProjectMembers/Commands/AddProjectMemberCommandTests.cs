using Projects.Application.Common.Models.Dto;
using Projects.Application.ProjectMembers.Commands;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Enums;
using Projects.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Projects.IntegrationTests.Application.ProjectMembers.Commands;
[Collection("Base")]
public class AddProjectMemberCommandTests: IAsyncLifetime
{
    private readonly Base _base;
    public AddProjectMemberCommandTests(Base @base)
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
    public async Task AddProjectMemberCommand_ValidData_ReturnsProjectMemberDto()
    {
        //arrange
        var memberCreator = new ProjectMember("testUserId", "testUserEmail@test.com", null, ProjectMemberType.Leader, InvitationStatus.Accepted);
        var projects = new List<Project>()
        {
            new Project("testProject","",memberCreator)
        };

        _base._factory.SeedData<Program, ApplicationDbContext, Project>(projects);

        _base._client.SetHeaders("testUserId", "testUserEmail@test.com");

        AddProjectMemberCommand command = new("testUserToAddId","testUserToAddEmail@test.com",null, ProjectMemberType.Member, projects[0].Id);

        //act
        var response = await _base._client.PostAsync($"api/Projects/{command.ProjectId}/projectMembers/addMember", command.ToStringContent());

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returnedProjectMemberDto = JsonSerializer.Deserialize<ProjectMemberDto>(responseString, options);


        Assert.NotNull(returnedProjectMemberDto);
        Assert.Equal(InvitationStatus.Invited, returnedProjectMemberDto.InvitationStatus);
        Assert.Equal(ProjectMemberType.Member, returnedProjectMemberDto.Type);
    }




}
