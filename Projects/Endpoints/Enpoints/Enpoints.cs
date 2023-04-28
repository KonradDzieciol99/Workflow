using AutoMapper;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Projects.Models.Dto;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Projects.Common;
using Projects.Repositories;
using Projects.Entity;
using Projects.Models;
using Microsoft.Azure.Amqp.Framing;

namespace Projects.Endpoints.MapProjectMember
{
    public static class Enpoints
    {
        public static RouteGroupBuilder MapProjectMemberEnpoints(this RouteGroupBuilder group)
        {

            group.MapPost("/", async ([FromServices] IUnitOfWork unitOfWork,
                                        [FromServices] IAzureServiceBusSender azureServiceBusSender,
                                        [FromServices] IMapper mapper,
                                        ClaimsPrincipal user,
                                        HttpContext context,
                                        [FromBody] CreateProjectMemberDto createProjectMember,
                                        IAuthorizationService authorizationService
                                        ) =>
            {

                var userEmail = user.FindFirstValue(ClaimTypes.Email);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var authorizationResult = await authorizationService.AuthorizeAsync(user, createProjectMember.ProjectId, "ManagementPolicy");
                if (!authorizationResult.Succeeded)
                    return Results.Forbid();

                var newMember = mapper.Map<ProjectMember>(createProjectMember);

                unitOfWork.ProjectMemberRepository.Add(newMember);

                if (await unitOfWork.Complete())
                {
                    await azureServiceBusSender.PublishMessage(mapper.Map<ProjectMemberAddedEvent>(newMember));
                    return Results.Ok(newMember);
                }

                return Results.BadRequest("Error occurred during member creation.");

            })
            .AddEndpointFilter<ValidatorFilter<CreateProjectMemberDto>>();

            group.MapDelete("/", async ([FromServices] IUnitOfWork unitOfWork,
                            [FromServices] IAzureServiceBusSender azureServiceBusSender,
                            [FromServices] IMapper mapper,
                            ClaimsPrincipal user,
                            HttpContext context,
                            IAuthorizationService authorizationService,
                            [AsParameters] RemoveProjectMemberDto removeProjectMember) => //sprawdzić czy uzytkownik do usuniecie nie jest liderem
            {
                var userEmail = user.FindFirstValue(ClaimTypes.Email);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var authorizationResult = await authorizationService.AuthorizeAsync(user, removeProjectMember.ProjectId, "ManagementPolicy");
                if (!authorizationResult.Succeeded)
                    return Results.Forbid();

                var projectMember = await unitOfWork.ProjectMemberRepository.GetProjectMemberAsync(removeProjectMember.ProjectId, removeProjectMember.UserId);
                
                if (projectMember is null)
                    return Results.BadRequest("Such member does not exist.");

                var result = projectMember.CanBeDeleted();

                if (!result)
                    return Results.BadRequest("Project leader cannot be removed.");

                unitOfWork.ProjectMemberRepository.Remove(projectMember);

                if (await unitOfWork.Complete())
                {
                    await azureServiceBusSender.PublishMessage(mapper.Map<ProjectMemberRemovedEvent>(removeProjectMember));
                    return Results.NoContent();
                }

                return Results.BadRequest("Error occurred during member removing.");

            })
            .AddEndpointFilter<ValidatorFilter<RemoveProjectMemberDto>>();

            return group;
        }

        public static RouteGroupBuilder MapProjectsEnpoints(this RouteGroupBuilder group)
        {

            group.MapGet("/{id}", async ([FromServices] IUnitOfWork unitOfWork,
                                      [FromServices] IMapper mapper,
                                      ClaimsPrincipal user,
                                      [FromRoute] string id,
                                      HttpContext context,
                                      IAuthorizationService authorizationService) =>
            {
                var userEmail = user.FindFirstValue(ClaimTypes.Email);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var authorizationResult = await authorizationService.AuthorizeAsync(user, id, "MembershipPolicy");

                if (!authorizationResult.Succeeded)
                    return Results.Forbid();

                var project = await unitOfWork.ProjectRepository.GetOneByIdAsync(id);

                if (project is null)
                    return Results.BadRequest("Project cannot be found.");

                return Results.Ok(mapper.Map<ProjectDto>(project));
            });

            group.MapGet("/", async ([FromServices] IUnitOfWork unitOfWork,
                                      [FromServices] IMapper mapper,
                                      ClaimsPrincipal user,
                                      [AsParameters] AppParams @params) =>
            {
                var userEmail = user.FindFirstValue(ClaimTypes.Email);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId is null || userEmail is null)
                    return Results.BadRequest("User cannot be identified.");

                var result = await unitOfWork.ProjectMemberRepository.GetUserProjects(userId, @params);

                var projectsWithTotalCount = new ProjectsWithTotalCount()
                {
                    Count = result.TotalCount,
                    Result = mapper.Map<List<ProjectDto>>(result.Projects)
                };

                return Results.Ok(projectsWithTotalCount);

            })
            .AddEndpointFilter<ValidatorFilter<AppParams>>();

            group.MapPost("/", async ([FromServices] IUnitOfWork unitOfWork,
                                        [FromServices] IAzureServiceBusSender azureServiceBusSender,
                                        [FromServices] IMapper mapper,
                                        ClaimsPrincipal user,
                                        [FromBody] CreateProjectDto projectDto) =>
            {
                var userEmail = user.FindFirstValue(ClaimTypes.Email);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var userPhotUrl = user.FindFirstValue("picture");

                if (userId is null || userEmail is null)
                    return Results.BadRequest("User cannot be identified.");

                var member = new ProjectMember() { Type = ProjectMemberType.Leader, UserEmail = userEmail, UserId = userId, PhotoUrl = userPhotUrl };

                var project = new Project() { IconUrl = projectDto.Icon.Url, Name = projectDto.Name, ProjectMembers = new List<ProjectMember> { member } };

                unitOfWork.ProjectRepository.Add(project);

                if (await unitOfWork.Complete())
                {
                    await azureServiceBusSender.PublishMessage(mapper.Map<ProjectMemberAddedEvent>(member));
                    return Results.Ok(mapper.Map<ProjectDto>(project));
                }

                return Results.BadRequest("Error occurred during project creation.");

            })
            .AddEndpointFilter<ValidatorFilter<CreateProjectDto>>();

            group.MapDelete("/{id}", async ([FromServices] IUnitOfWork unitOfWork,
                                        [FromServices] IMapper mapper,
                                        ClaimsPrincipal user,
                                        HttpContext context,
                                        IAuthorizationService authorizationService,
                                        [FromRoute] string id) =>
            {
                var userEmail = user.FindFirstValue(ClaimTypes.Email);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId is null || userEmail is null)
                    return Results.BadRequest("User cannot be identified.");
                if (id is null)
                    return Results.BadRequest("Enter the ID of the project to be deleted.");

                var authorizationResult = await authorizationService.AuthorizeAsync(user, id, "AuthorPolicy");

                if (!authorizationResult.Succeeded)
                    return TypedResults.Forbid();

                var resoult = await unitOfWork.ProjectRepository.ExecuteDeleteAsync(id);

                if (resoult > 0)
                    return Results.NoContent();

                return Results.BadRequest("Project could not be deleted.");
            });
            return group;
        }

        //public static RouteGroupBuilder MapSellersApi(this RouteGroupBuilder group)
        //{
        //    group.MapGet("/", GetAllSellers);
        //    return group;
        //}
        //public static Ok<List<Seller>> GetAllSellers(SellerDb db)
        //{
        //    var sellers = db.Sellers;
        //    return TypedResults.Ok(sellers.ToList());
        //}
    }
}
