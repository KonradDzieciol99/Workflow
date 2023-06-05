﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Exceptions;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models.Dto;
using Projects.Application.ProjectMembers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Projects.Queries;

public record GetProjectQuery(string ProjectId) : IAuthorizationRequest<ProjectDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() =>
        new List<IAuthorizationRequirement> { new ProjectMembershipRequirement(ProjectId) };
}

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProjectQueryHandler(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        this._mapper = mapper;
    }

    public async Task<ProjectDto> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ReadOnlyProjectRepository.GetOneAsync(request.ProjectId);

        if (project is null)
            throw new BadRequestException("Project cannot be found.");

        return _mapper.Map<ProjectDto>(project);
    }
}

