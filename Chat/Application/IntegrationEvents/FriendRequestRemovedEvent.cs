using MessageBus;

namespace Chat.Application.IntegrationEvents;

public record FriendRequestRemovedEvent(string ActionInitiatorUserId, string ActionInitiatorUserEmail, string? ActionInitiatorUserPhotoUrl, string FriendToRemoveUserId, string FriendToRemoveUserEmail, string? FriendToRemoveUserPhotoUrl) : IntegrationEvent;