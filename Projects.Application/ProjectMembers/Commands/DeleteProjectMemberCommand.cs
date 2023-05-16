﻿using AutoMapper;
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
    public async Task<ProjectMemberDto> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var newMember = new ProjectMember(request.UserId, request.UserEmail, request.PhotoUrl, request.Type, request.ProjectId);

        newMember.AddProjectMember();

        _unitOfWork.ProjectMemberRepository.Add(newMember);

        await _unitOfWork.Complete();

        var projectMemberDto = new ProjectMemberDto(newMember.Id, newMember.UserId, newMember.UserEmail, newMember.Type, newMember.ProjectId);

        return projectMemberDto;
    }

    public async Task Handle(DeleteProjectMemberCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

