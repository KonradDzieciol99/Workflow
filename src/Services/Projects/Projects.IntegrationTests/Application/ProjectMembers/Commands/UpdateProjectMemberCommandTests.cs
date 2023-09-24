using Projects.Application.ProjectMembers.Commands;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Enums;
using Projects.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestsHelpers.Extensions;

namespace Projects.IntegrationTests.Application.ProjectMembers.Commands;

[Collection("Base")]
public class UpdateProjectMemberCommandTests : IAsyncLifetime
{
    private readonly Base _base;

    public UpdateProjectMemberCommandTests(Base @base)
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
    public async Task UpdateProjectMemberCommand_ValidData_ReturnsNoContent()
    {
        //arrange
        var memberCreator = new ProjectMember(
            "testUserId",
            "testUserEmail@test.com",
            null,
            ProjectMemberType.Leader,
            InvitationStatus.Accepted
        );
        var projects = new List<Project>() { new Project("testProject", "", memberCreator) };

        var invitedMember = new ProjectMember(
            "testUserId2",
            "testUserEmail@test.com2",
            null,
            ProjectMemberType.Member,
            InvitationStatus.Accepted
        );
        projects[0].AddProjectMember(invitedMember);

        _base._factory.SeedData<Program, ApplicationDbContext, Project>(projects);

        _base._client.SetHeaders("testUserId", "testUserEmail@test.com");

        UpdateProjectMemberCommand command =
            new(ProjectMemberType.Admin, projects[0].Id, invitedMember.UserId);

        //act
        var response = await _base._client.PutAsync(
            $"api/Projects/{command.ProjectId}/projectMembers/{invitedMember.UserId}",
            command.ToStringContent()
        );

        //assert
        var invitedMemverAfterAction = await _base._factory.FindAsync<
            Program,
            ApplicationDbContext,
            ProjectMember
        >(invitedMember.Id);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(ProjectMemberType.Admin, invitedMemverAfterAction.Type);
    }
}
