using MessageBus;

namespace IdentityDuende.Application.IntegrationEvents;

public record RegistrationEvent(string Email, string Token, string UserId, string? PhotoUrl)
    : IntegrationEvent;
