using Chat.Application.Common.Authorization;
using Chat.Application.Common.Models;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using AutoMapper;
using System.Collections.Generic;

namespace Chat.Application.FriendRequests.Queries;

public record GetConfirmedFriendRequestsQuery : IAuthorizationRequest<List<FriendRequestDto>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new List<IAuthorizationRequirement>();
}
public class GetConfirmedFriendRequestsQueryHandler : IRequestHandler<GetConfirmedFriendRequestsQuery, List<FriendRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetConfirmedFriendRequestsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
    }
    public async Task<List<FriendRequestDto>> Handle(GetConfirmedFriendRequestsQuery request, CancellationToken cancellationToken)
    {
        var friendRequests = await _unitOfWork.FriendRequestRepository.GetConfirmedAsync(_currentUserService.UserId);

        return _mapper.Map<List<FriendRequestDto>>(friendRequests);

    }

}