using Chat.Application.Common.Authorization.Requirements;
using Chat.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Chat.Application.Common.Authorization.Handlers;

public class ShareFriendRequestRequirementHandler
    : AuthorizationHandler<ShareFriendRequestRequirement>
{
    private readonly IUnitOfWork _unitOfWork;

    public ShareFriendRequestRequirementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ShareFriendRequestRequirement requirement
    )
    {
        var userId =
            context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException(
                $"User identifier claim '{nameof(ClaimTypes.NameIdentifier)}' not found in context."
            );
        var targetUserId =
            requirement.TargetUserId
            ?? throw new InvalidOperationException(
                $"{nameof(requirement.TargetUserId)} is null in requirement."
            );

        var result = await _unitOfWork.FriendRequestRepository.CheckIfTheyShareFriendRequest(
            userId,
            targetUserId
        );

        if (result)
            context.Succeed(requirement);

        await Task.CompletedTask;
        return;
    }
}
