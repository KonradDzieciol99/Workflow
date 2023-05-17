using AutoMapper;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Models;
using Projects.Application.Common.Models.Dto;
using Projects.Application.ProjectMembers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Application.Projects.Commands;

public record CreateProjectCommand(string Name, Icon Icon) : IAuthorizationRequest<ProjectDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        throw new NotImplementedException();
    }
}

//public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public CreateProjectCommandHandler(IUnitOfWork unitOfWork)
//    {
//        _unitOfWork = unitOfWork;
//    }
//    public Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
//    {
//        //var userEmail = user.FindFirstValue(ClaimTypes.Email);
//        //var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
//        //var userPhotUrl = user.FindFirstValue("picture");

//        ////if (userId is null || userEmail is null)
//        ////    return Results.BadRequest("User cannot be identified.");

//        //var member = new ProjectMember() { Type = ProjectMemberType.Leader, UserEmail = userEmail, UserId = userId, PhotoUrl = userPhotUrl };

//        //var project = new Project() { IconUrl = projectDto.Icon.Url, Name = projectDto.Name, ProjectMembers = new List<ProjectMember> { member } };
//        //unitOfWork.ProjectRepository.Add(project);

//        //if (await unitOfWork.Complete())
//        //{
//        //    await azureServiceBusSender.PublishMessage(mapper.Map<ProjectMemberAddedEvent>(member));
//        //    return Results.Ok(mapper.Map<ProjectDto>(project));
//        //}

//        //return Results.BadRequest("Error occurred during project creation.");
//    }
//}
