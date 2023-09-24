using Chat.Application.Common.Models;
using MessageBus;

namespace Chat.Application.IntegrationEvents;

public record UserOnlineFriendsAndUnMesUserEmailsEvent(
    UserDto OnlineUser,
    List<UserDto> ListOfAcceptedFriends,
    List<string> UnreadMessagesUserEmails
) : IntegrationEvent;
