﻿using Chat.Application.Common.Authorization;
using Chat.Application.Common.Models;
using Chat.Application.IntegrationEvents;
using Chat.Domain.Entity;
using Chat.Infrastructure.Repositories;
using HttpMessage.Authorization;
using HttpMessage.Services;
using MediatR;
using MessageBus;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.FriendRequests.Commands;

public record CreateFriendRequestCommand(
    string TargetUserId,
    string TargetUserEmail,
    string? TargetUserPhotoUrl
) : IAuthorizationRequest<FriendRequestDto>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement() => new();
}

public class CreateFriendRequestCommandHandler
    : IRequestHandler<CreateFriendRequestCommand, FriendRequestDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventBusSender _azureServiceBusSender;

    public CreateFriendRequestCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEventBusSender azureServiceBusSender
    )
    {
        _unitOfWork = unitOfWork;
        this._currentUserService = currentUserService;
        this._azureServiceBusSender = azureServiceBusSender;
    }

    public async Task<FriendRequestDto> Handle(
        CreateFriendRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var friendRequest = new FriendRequest(
            _currentUserService.GetUserId(),
            _currentUserService.GetUserEmail(),
            _currentUserService.GetUserPhoto(),
            request.TargetUserId,
            request.TargetUserEmail,
            request.TargetUserPhotoUrl
        );

        _unitOfWork.FriendRequestRepository.Add(friendRequest);

        await _unitOfWork.Complete();

        var friendInvitationAddedEvent = new FriendRequestAddedEvent(
            friendRequest.InviterUserId,
            friendRequest.InviterUserEmail,
            friendRequest.InviterPhotoUrl,
            friendRequest.InvitedUserId,
            friendRequest.InvitedUserEmail,
            friendRequest.InvitedPhotoUrl
        );

        await _azureServiceBusSender.PublishMessage(friendInvitationAddedEvent);

        return new FriendRequestDto(
            friendRequest.InviterUserId,
            friendRequest.InviterUserEmail,
            friendRequest.InviterPhotoUrl,
            friendRequest.InvitedUserId,
            friendRequest.InvitedUserEmail,
            friendRequest.InvitedPhotoUrl,
            friendRequest.Confirmed
        );
    }
}
