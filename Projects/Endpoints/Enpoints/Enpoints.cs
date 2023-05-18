using AutoMapper;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Projects.Common;
using Projects.Application.Common.Models.Dto;
using Projects.Application.Common.Models;
using Projects.Domain.Interfaces;
using MediatR;
using Projects.Application.ProjectMembers.Commands;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Projects.Application.Projects.Queries;
using Projects.Application.Projects.Commands;
using Microsoft.AspNetCore.Http;

namespace Projects.Endpoints.Enpoints
{
    public static class Enpoints
    {
        public static RouteGroupBuilder MapProjectMemberEnpoints(this RouteGroupBuilder group)
        {

            group.MapPost("/{projectId}/projectMembers", async (
                                        [FromRoute] string projectId,
                                        [FromServices] IMediator mediator,
                                        [FromBody] AddProjectMemberCommand command
                                        ) =>
            {
                if (projectId != command.ProjectId)
                    return Results.BadRequest();

                return Results.Ok(await mediator.Send(command));
            });

            group.MapDelete("/{projectId}/projectMembers/{projectMemberId}", async (
                            [FromServices] IMediator mediator,
                            [FromRoute] string projectMemberId,
                            [FromRoute] string projectId,
                            [FromBody] DeleteProjectMemberCommand command) =>
            {
                if (projectId != command.ProjectId && projectMemberId != command.UserId)
                    return Results.BadRequest();

                await mediator.Send(command);

                return Results.NoContent();

            });

            //    group.MapPut("/{projectId}/projectMembers/{projectMemberId}", async ([FromServices] IUnitOfWork unitOfWork,
            //        [FromServices] IAzureServiceBusSender azureServiceBusSender,
            //        [FromServices] IMapper mapper,
            //        ClaimsPrincipal user,
            //        [FromServices] IAuthorizationService authorizationService,
            //        [FromRoute] string projectMemberId,
            //        [FromRoute] string projectId,
            //        [FromBody] UpdateProjectMemberDto UpdateProjectMemberDto) =>
            //    {
            //        var userEmail = user.FindFirstValue(ClaimTypes.Email);
            //        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            //        var authorizationResult = await authorizationService.AuthorizeAsync(user, projectId, "ManagementPolicy");
            //        if (!authorizationResult.Succeeded)
            //            return Results.Forbid();

            //        var projectMember = await unitOfWork.ProjectMemberRepository.GetAsync(projectMemberId);

            //        if (projectMember is null)
            //            return Results.BadRequest("Such member does not exist.");

            //        var result = projectMember.CanBeDeleted();

            //        if (!result)
            //            return Results.BadRequest("Project leader cannot be removed.");

            //        unitOfWork.ProjectMemberRepository.Remove(projectMember);

            //        if (await unitOfWork.Complete())
            //        {
            //            await azureServiceBusSender.PublishMessage(new ProjectMemberRemovedEvent() { ProjectMemberId = projectMemberId });
            //            return Results.NoContent();
            //        }

            //        return Results.BadRequest("Error occurred during member removing.");

            //    });

            return group;
        }

        public static RouteGroupBuilder MapProjectsEnpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/{projectId}", async ([FromServices] IMediator mediator,[FromRoute] string projectId) =>
            {
                return await mediator.Send(new GetProjectQuery(projectId));
            });

            group.MapGet("/", async ([FromServices] IMediator mediator, [AsParameters] AppParams @params) =>
            {
                return await mediator.Send(new GetProjectsQuery(@params));
            });

            group.MapPost("/", async ([FromServices] IMediator mediator, [FromBody] CreateProjectCommand command, HttpContext httpContext) =>
            {
                var result = await mediator.Send(command);

                var url = httpContext.Request.Path + "/" + result.Id;

                return Results.Created(url, result);

            });

            group.MapDelete("/{projectId}", async ([FromServices] IMediator mediator, [FromRoute] string projectId) =>
            {
                await mediator.Send(new DeleteProjectCommand(projectId));

                return Results.NoContent();
            });

            return group;
        }
    }
}
