using Docker.DotNet.Models;
using Org.BouncyCastle.Crypto;
using Projects.Application.Common.Models;
using Projects.Application.ProjectMembers.Queries;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Enums;
using Projects.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Projects.IntegrationTests.Application.ProjectMembers.Queries;
[Collection("Base")]
public class GetMembersStatusesQueryTests: IAsyncLifetime
{
    private readonly Base _base;
    public GetMembersStatusesQueryTests(Base @base)
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

    public static IEnumerable<object[]> GetMembersStatusesQueryList => new List<object[]>
    {
        new object[]{ new GetMembersStatusesQuery("id",new List<string>{"dd","sss","sssss","fdsfgd"}),MemberStatusType.Uninvited },
        new object[]{ new GetMembersStatusesQuery("id",new List<string>{ "testUserId2", "testUserId3" }),MemberStatusType.Invited },
    };

    [Theory]
    [MemberData(nameof(GetMembersStatusesQueryList))]
    public async Task GetMembersStatusesQuery_ValidData_ReturnsOkAndStatuses(GetMembersStatusesQuery baseQuery ,MemberStatusType status)
    {
        //arrange
        var memberCreator = new ProjectMember("testUserId", "testUserEmail@test.com", null, ProjectMemberType.Leader, InvitationStatus.Accepted);
        var projects = new List<Project>()
        {
            new Project("testProject","",memberCreator)
        };

        var invitedMember = new ProjectMember("testUserId2", "testUserEmail@test.com2", null, ProjectMemberType.Member, InvitationStatus.Invited);
        projects[0].AddProjectMember(invitedMember);
        var invitedMember2 = new ProjectMember("testUserId3", "testUserEmail@test.com3", null, ProjectMemberType.Member, InvitationStatus.Invited);
        projects[0].AddProjectMember(invitedMember2);
        _base._factory.SeedData<Program, ApplicationDbContext, Project>(projects);
        _base._client.SetHeaders("testUserId", "testUserEmail@test.com");
        var query = baseQuery with { projectId = projects[0].Id };

        //act
        var response = await _base._client.GetAsync($"api/Projects/{query.projectId}/GetMembersStatuses?usersIds={string.Join("&usersIds=", query.UsersIds)}");

        //assert
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var returned = JsonSerializer.Deserialize<List<MemberStatusDto>>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.All(returned, item => Assert.Equal(status, item.Status));
    }
}
