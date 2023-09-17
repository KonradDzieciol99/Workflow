using AutoMapper;
using Chat.Application.Common.Authorization;
using Chat.Application.Common.Models;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.FriendRequests.Queries;

public record GetConfirmedFriendRequestsQuery(int Skip, int Take, string? Search) : IAuthorizationRequest<List<FriendRequestDto>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new();
}
public class GetConfirmedFriendRequestsQueryHandler : IRequestHandler<GetConfirmedFriendRequestsQuery, List<FriendRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetConfirmedFriendRequestsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
    }
    public async Task<List<FriendRequestDto>> Handle(GetConfirmedFriendRequestsQuery request, CancellationToken cancellationToken)
    {
        var friendRequests = await _unitOfWork.FriendRequestRepository.GetConfirmedAsync(_currentUserService.GetUserId(), request);

        return _mapper.Map<List<FriendRequestDto>>(friendRequests);

    }
}