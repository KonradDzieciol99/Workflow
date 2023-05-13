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
using Projects.Domain.Entities;
using Projects.Application.Common.ServiceInterfaces;
using Projects.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Models.Dto;

namespace Projects.Application.ProjectMembers.Commands;

public record AddProjectMemberCommand(string UserId,
                        string UserEmail,
                        string PhotoUrl,
                        ProjectMemberType Type,
                        string ProjectId) : IRequest<ProjectMemberDto>, IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>() { new ProjectManagementRequirement() { ProjectId = ProjectId } };
    }
}

public class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberCommand, ProjectMemberDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectMemberDto> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var newMember = new ProjectMember(request.UserId,request.UserEmail,request.PhotoUrl,request.Type,request.ProjectId);

        _unitOfWork.ProjectMemberRepository.Add(newMember);

        await _unitOfWork.Complete();

        var projectMemberDto = new ProjectMemberDto(newMember.Id, newMember.UserId, newMember.UserEmail, newMember.Type, newMember.ProjectId);
        
        return projectMemberDto;
    }
}


