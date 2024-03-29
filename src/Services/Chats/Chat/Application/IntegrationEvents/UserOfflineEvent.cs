﻿using Chat.Application.Common.Models;
using MessageBus;
using Chat.Infrastructure.Repositories;
using MediatR;

namespace Chat.Application.IntegrationEvents;

public record UserOfflineEvent(UserDto User) : IntegrationEvent;

public class UserOfflineEventHandler : IRequestHandler<UserOfflineEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBusSender _azureServiceBusSender;

    public UserOfflineEventHandler(IUnitOfWork unitOfWork, IEventBusSender _azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork;
        this._azureServiceBusSender = _azureServiceBusSender;
    }

    public async Task Handle(UserOfflineEvent request, CancellationToken cancellationToken)
    {
        var confirmed = await _unitOfWork.FriendRequestRepository.GetConfirmedAsync(
            request.User.Id
        );

        var listOfAcceptedFriends = confirmed
            .Select(
                fr =>
                    fr.InviterUserEmail != request.User.Email
                        ? new UserDto(fr.InviterUserId, fr.InviterUserEmail, fr.InviterUserEmail)
                        : new UserDto(fr.InvitedUserId, fr.InvitedUserEmail, fr.InvitedUserEmail)
            )
            .ToList();

        var @event = new UserOfflineWithFriendsEvent(request.User, listOfAcceptedFriends);

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
