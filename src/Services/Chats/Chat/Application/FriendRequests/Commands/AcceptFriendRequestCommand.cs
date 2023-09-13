using Chat.Application.Common.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Application.Common.Exceptions;
using Chat.Application.IntegrationEvents;
using Chat.Domain.Common.Exceptions;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using MediatR;
using MessageBus;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.FriendRequests.Commands;

public record AcceptFriendRequestCommand(string TargetUserId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>() { new ShareFriendRequestRequirement(TargetUserId) };
    }
}
public class AcceptFriendRequestCommandHandler : IRequestHandler<AcceptFriendRequestCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventBusSender _azureServiceBusSender;

    public AcceptFriendRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IEventBusSender azureServiceBusSender)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
    }
    public async Task Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _unitOfWork.FriendRequestRepository.GetAsync(_currentUserService.GetUserId(), request.TargetUserId) ?? throw new ChatDomainException("Friend request cannot be found.", new NotFoundException());

        friendRequest.AcceptRequest(_currentUserService.GetUserId());

        await _unitOfWork.Complete();

        var @event = new FriendRequestAcceptedEvent(friendRequest.InviterUserId,
                                                    friendRequest.InviterUserEmail,
                                                    friendRequest.InviterPhotoUrl,
                                                    friendRequest.InvitedUserId,
                                                    friendRequest.InvitedUserEmail,
                                                    friendRequest.InvitedPhotoUrl);

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
