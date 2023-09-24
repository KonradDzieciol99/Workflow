using MessageBus;

namespace Tasks.Application.IntegrationEvents;

public record TaskAddedEvent(
    string Id,
    string Name,
    string? Description,
    string ProjectId,
    string? TaskAssigneeMemberId,
    int Priority,
    int State,
    DateTime DueDate,
    DateTime StartDate,
    string? TaskLeaderId
) : IntegrationEvent;
