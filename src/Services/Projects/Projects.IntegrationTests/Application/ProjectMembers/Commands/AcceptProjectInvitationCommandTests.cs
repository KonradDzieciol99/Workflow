using Projects.Application.ProjectMembers.Commands;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Enums;
using Projects.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestsHelpers.Extensions;

namespace Projects.IntegrationTests.Application.ProjectMembers.Commands;

[Collection("Base")]
public class AcceptProjectInvitationCommandTests : IAsyncLifetime
{
    private readonly Base _base;

    public AcceptProjectInvitationCommandTests(Base @base)
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
    public async Task AcceptProjectInvitationCommand_ValidData_ReturnsNoContent()
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

        var memberToInvite = new ProjectMember(
            "testUserId2",
            "testUserEmail@test.com2",
            null,
            ProjectMemberType.Leader,
            InvitationStatus.Invited
        );
        projects[0].AddProjectMember(memberToInvite);

        _base._factory.SeedData<Program, ApplicationDbContext, Project>(projects);

        _base._client.SetHeaders("testUserId2", "testUserEmail@test.com2");

        AcceptProjectInvitationCommand command = new(projects[0].Id);

        //act
        var response = await _base._client.PutAsync(
            $"api/Projects/{command.ProjectId}/AcceptInvitation",
            null
        );

        //assert
        var invitedMemverAfterInvitationAction = await _base._factory.FindAsync<
            Program,
            ApplicationDbContext,
            ProjectMember
        >(memberToInvite.Id);

        Assert.NotNull(invitedMemverAfterInvitationAction);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(
            InvitationStatus.Accepted,
            invitedMemverAfterInvitationAction.InvitationStatus
        );
    }
}
