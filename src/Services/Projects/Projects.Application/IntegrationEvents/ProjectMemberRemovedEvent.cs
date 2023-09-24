using MessageBus;

namespace Projects.Application.IntegrationEvents;

public record ProjectMemberRemovedEvent(string ProjectMemberId, string ProjectId, string UserId)
    : IntegrationEvent;
