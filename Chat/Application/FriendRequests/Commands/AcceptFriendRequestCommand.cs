using Chat.Application.Common.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Application.Common.Models;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Chat.Infrastructure.Repositories;
using Chat.Services;

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
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public AcceptFriendRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAzureServiceBusSender azureServiceBusSender)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
    }
    public async Task Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _unitOfWork.FriendRequestRepository.GetAsync(_currentUserService.UserId, request.TargetUserId);

        friendRequest.AcceptRequest(_currentUserService.UserId);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

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
