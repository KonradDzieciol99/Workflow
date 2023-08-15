using Chat.Application.Common.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Application.Common.Models;
using Chat.Domain.Entity;
using MediatR;
using MessageBus.Events;
using MessageBus;
using Microsoft.AspNetCore.Authorization;
using Chat.Infrastructure.Repositories;
using Chat.Services;

namespace Chat.Application.FriendRequests.Commands;

public record DeleteFriendRequestCommand(string TargetUserId) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>(){new ShareFriendRequestRequirement(TargetUserId)};
    }
}
public class DeleteFriendRequestCommandHandler : IRequestHandler<DeleteFriendRequestCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAzureServiceBusSender _azureServiceBusSender;

    public DeleteFriendRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAzureServiceBusSender azureServiceBusSender)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService)); 
        this._azureServiceBusSender = azureServiceBusSender ?? throw new ArgumentNullException(nameof(azureServiceBusSender));
    }
    public async Task Handle(DeleteFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _unitOfWork.FriendRequestRepository.GetAsync(_currentUserService.GetUserId(), request.TargetUserId);

        _unitOfWork.FriendRequestRepository.Remove(friendRequest);

        if (!await _unitOfWork.Complete())
            throw new InvalidOperationException();

        // Domain logic
        
        if (friendRequest.InviterUserId == _currentUserService.GetUserId() && friendRequest.Confirmed == false)
        {
            var friendRequestCanceledEvent = new FriendRequestCanceledEvent(friendRequest.InviterUserId,
                                                                    friendRequest.InviterUserEmail,
                                                                    friendRequest.InviterPhotoUrl,
                                                                    friendRequest.InvitedUserId,
                                                                    friendRequest.InvitedUserEmail,
                                                                    friendRequest.InvitedPhotoUrl);
            await _azureServiceBusSender.PublishMessage(friendRequestCanceledEvent);
            return;
        }else if(friendRequest.InvitedUserId == _currentUserService.GetUserId() && friendRequest.Confirmed == false)
        {
            var friendRequestDeclinedEvent = new FriendRequestDeclinedEvent(friendRequest.InviterUserId,
                                                        friendRequest.InviterUserEmail,
                                                        friendRequest.InviterPhotoUrl,
                                                        friendRequest.InvitedUserId,
                                                        friendRequest.InvitedUserEmail,
                                                        friendRequest.InvitedPhotoUrl);
            await _azureServiceBusSender.PublishMessage(friendRequestDeclinedEvent);
            return;
        }

        var @event = new FriendRequestRemovedEvent(friendRequest.InviterUserId,
                                                                    friendRequest.InviterUserEmail,
                                                                    friendRequest.InviterPhotoUrl,
                                                                    friendRequest.InvitedUserId,
                                                                    friendRequest.InvitedUserEmail,
                                                                    friendRequest.InvitedPhotoUrl);
        //
        await _azureServiceBusSender.PublishMessage(@event);

       
        return;
    }
}