using AutoMapper;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Application.Common.ServiceInterfaces;
using Projects.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Models.Dto;
using Projects.Domain.AggregatesModel.ProjectAggregate;

namespace Projects.Application.ProjectMembers.Commands;

public record DeleteProjectMemberCommand(string UserId,string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>() 
        { 
            new ProjectMembershipRequirement(ProjectId),
            new ProjectManagementRequirement(ProjectId)
        };
        return listOfRequirements;
    }
}

public class DeleteProjectMemberCommandHandler : IRequestHandler<DeleteProjectMemberCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteProjectMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.RemoveProjectMember(request.UserId);

        await _unitOfWork.Complete();
    }
}


