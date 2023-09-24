using MessageBus;

namespace Projects.Application.IntegrationEvents;

public record ProjectMemberUpdatedEvent(
    string? PhotoUrl,
    string ProjectMemberId,
    string UserId,
    string UserEmail,
    int Type,
    int InvitationStatus,
    string ProjectId
) : IntegrationEvent;
