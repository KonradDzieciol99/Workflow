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

namespace Projects.Application.Projects.Commands;

public record CreateProjectCommand(string Name, Icon Icon) : IAuthorizationRequest<ProjectDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement> { };
}
//public class CreateProjectCommand : IAuthorizationRequest<ProjectDto>
//{
//    public string Name { get; set; }
//    public Icon Icon { get; set; }
//    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement> { };
//}
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateProjectCommandHandler(IUnitOfWork unitOfWork,ICurrentUserService currentUserService,IMapper mapper)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(_currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
    }
    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project(request.Name,request.Icon.Url){};

        var member = new ProjectMember(_currentUserService.UserId, _currentUserService.UserEmail,_currentUserService.UserPhoto, ProjectMemberType.Leader);

        project.AddProjectMember(member);

        _unitOfWork.ProjectRepository.Add(project);

        await _unitOfWork.Complete();

        return _mapper.Map<ProjectDto>(project);
    }
}