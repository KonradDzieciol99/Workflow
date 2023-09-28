using AutoMapper;
using HttpMessage.Authorization;
using HttpMessage.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models;
using Projects.Application.Common.Models.Dto;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Enums;

namespace Projects.Application.Projects.Commands;

public record CreateProjectCommand(string Name, Icon Icon) : IAuthorizationRequest<ProjectDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new();
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ProjectDto> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken
    )
    {
        var member = new ProjectMember(
            _currentUserService.GetUserId(),
            _currentUserService.GetUserEmail(),
            _currentUserService.GetUserPhoto(),
            ProjectMemberType.Leader,
            InvitationStatus.Accepted
        );

        var project = new Project(request.Name, request.Icon.Url, member);

        _unitOfWork.ProjectRepository.Add(project);

        await _unitOfWork.Complete();

        return _mapper.Map<ProjectDto>(project);
    }
}
