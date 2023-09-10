using MessageBus;

namespace Projects.Application.IntegrationEvents;
public record ProjectMemberAddedEvent(string ProjectMemberId, string UserId, string UserEmail, string? PhotoUrl, int Type, string ProjectId, int InvitationStatus, string ProjectName, string projectIconUrl, bool IsNewProjectCreator) : IntegrationEvent;
