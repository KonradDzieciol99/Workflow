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
using Microsoft.Azure.Amqp.Encoding;
using Azure.Core;
using Projects.Application.Common.Interfaces;
using Microsoft.Azure.Amqp.Framing;
using Projects.Application.Common.Authorization.Requirements;

namespace Projects.Application.Projects.Commands;

public record DeleteProjectCommand(string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() =>
        new List<IAuthorizationRequirement> 
        {
            new ProjectMembershipRequirement(ProjectId),
            new ProjectAuthorRequirement(ProjectId)
        };
}

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork,ICurrentUserService currentUserService,IMapper mapper)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(_currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.RemoveProject();

        _unitOfWork.ProjectRepository.Remove(project);

        await _unitOfWork.Complete();
    }
}