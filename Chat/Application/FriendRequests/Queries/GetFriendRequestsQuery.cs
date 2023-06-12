using AutoMapper;
using Chat.Application.Common.Authorization;
using Chat.Application.Common.Models;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.FriendRequests.Queries;

public record GetFriendRequestsQuery() : IAuthorizationRequest<List<FriendRequestDto>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement>();

}
public class GetFriendRequestsHandler : IRequestHandler<GetFriendRequestsQuery, List<FriendRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetFriendRequestsHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<List<FriendRequestDto>> Handle(GetFriendRequestsQuery request, CancellationToken cancellationToken)
    {
        var friendRequests = await _unitOfWork.FriendRequestRepository.GetAsync(_currentUserService.UserId);

        return _mapper.Map<List<FriendRequestDto>>(friendRequests);

    }
}
