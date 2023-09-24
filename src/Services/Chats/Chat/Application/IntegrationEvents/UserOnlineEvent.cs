using Chat.Application.Common.Models;
using MediatR;
using MessageBus;
using Chat.Infrastructure.Repositories;

namespace Chat.Application.IntegrationEvents;

public record UserOnlineEvent(UserDto OnlineUser) : IntegrationEvent;

public class UserOnlineEventHandler : IRequestHandler<UserOnlineEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBusSender _azureServiceBusSender;

    public UserOnlineEventHandler(IUnitOfWork unitOfWork, IEventBusSender _azureServiceBusSender)
    {
        this._unitOfWork = unitOfWork;
        this._azureServiceBusSender = _azureServiceBusSender;
    }

    public async Task Handle(UserOnlineEvent request, CancellationToken cancellationToken)
    {
        var unreadMessagesUserEmails =
            await _unitOfWork.MessagesRepository.GetUnreadMessagesUserEmails(request.OnlineUser.Id);

        var confirmed = await _unitOfWork.FriendRequestRepository.GetConfirmedAsync(
            request.OnlineUser.Id
        );

        var listOfAcceptedFriends = confirmed
            .Select(
                fr =>
                    fr.InviterUserEmail != request.OnlineUser.Email
                        ? new UserDto(fr.InviterUserId, fr.InviterUserEmail, fr.InviterUserEmail)
                        : new UserDto(fr.InvitedUserId, fr.InvitedUserEmail, fr.InvitedUserEmail)
            )
            .ToList();

        var @event = new UserOnlineFriendsAndUnMesUserEmailsEvent(
            request.OnlineUser,
            listOfAcceptedFriends,
            unreadMessagesUserEmails
        );

        await _azureServiceBusSender.PublishMessage(@event);

        return;
    }
}
