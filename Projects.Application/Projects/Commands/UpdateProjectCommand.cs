using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Projects.Commands;

public record UpdateProjectCommand(string? Name, string? IconUrl, string? NewLeaderId, string ProjectId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>
        {
            new ProjectMembershipRequirement(ProjectId),
            new ProjectAuthorRequirement(ProjectId)
        };
    }
}
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(_currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
    }

    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        project.Update(request.Name, request.IconUrl, request.NewLeaderId);

        await _unitOfWork.Complete();
    }
}