using Chat.Application.Common.Authorization;
using Chat.Application.Common.Models;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.FriendRequests.Queries;

public record GetFriendsStatusQuery(List<string> UsersIds) : IAuthorizationRequest<List<FriendStatusDto>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new();
}
public class GetFriendsStatusQueryHandler : IRequestHandler<GetFriendsStatusQuery, List<FriendStatusDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetFriendsStatusQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<List<FriendStatusDto>> Handle(GetFriendsStatusQuery request, CancellationToken cancellationToken)
    {
        var friendRequests = await _unitOfWork.FriendRequestRepository.CheckUsersToUserStatusAsync(_currentUserService.GetUserId(), request.UsersIds);
        return friendRequests;
    }
}