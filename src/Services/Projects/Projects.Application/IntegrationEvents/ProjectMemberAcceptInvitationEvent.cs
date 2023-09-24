using MessageBus;

namespace Projects.Application.IntegrationEvents;

public record ProjectMemberAcceptInvitationEvent(
    string ProjectMemberId,
    string UserId,
    string UserEmail,
    string? PhotoUrl,
    int Type,
    string ProjectId,
    int InvitationStatus,
    string ProjectName,
    string projectIconUrl
) : IntegrationEvent;
