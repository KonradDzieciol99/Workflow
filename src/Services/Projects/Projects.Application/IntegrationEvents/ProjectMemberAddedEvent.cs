using MessageBus;
using Projects.Domain.Common.Enums;

namespace Projects.Application.IntegrationEvents;

public record ProjectMemberAddedEvent(
    string ProjectMemberId,
    string UserId,
    string UserEmail,
    string? PhotoUrl,
    ProjectMemberType Type,
    string ProjectId,
    InvitationStatus InvitationStatus,
    string ProjectName,
    string ProjectIconUrl,
    bool IsNewProjectCreator
) : IntegrationEvent;
