using MediatR;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Application.Common.Interfaces;
using Projects.Application.Common.Models.Dto;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Enums;

namespace Projects.Application.ProjectMembers.Commands;

public record AddProjectMemberCommand(
    string UserId,
    string UserEmail,
    string? PhotoUrl,
    ProjectMemberType Type,
    string ProjectId
) : IAuthorizationRequest<ProjectMemberDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        var listOfRequirements = new List<IAuthorizationRequirement>()
        {
            new ProjectMembershipRequirement(ProjectId),
            new ProjectManagementRequirement(ProjectId) // TODO zmienić na Domenowy serwis albo logikę domenową
        };
        return listOfRequirements;
    }
}

public class AddProjectMemberCommandHandler
    : IRequestHandler<AddProjectMemberCommand, ProjectMemberDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ProjectMemberDto> Handle(
        AddProjectMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var project = await _unitOfWork.ProjectRepository.GetOneAsync(request.ProjectId);

        var newMember = new ProjectMember(
            request.UserId,
            request.UserEmail,
            request.PhotoUrl,
            request.Type,
            InvitationStatus.Invited
        );

        project.AddProjectMember(newMember);

        await _unitOfWork.Complete();

        var projectMemberDto = new ProjectMemberDto(
            newMember.Id,
            newMember.UserId,
            newMember.UserEmail,
            newMember.Type,
            newMember.InvitationStatus,
            newMember.ProjectId
        );

        return projectMemberDto;
    }
}
