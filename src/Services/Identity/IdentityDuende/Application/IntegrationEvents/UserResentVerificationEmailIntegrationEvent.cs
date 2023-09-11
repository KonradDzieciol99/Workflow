using MessageBus;

namespace IdentityDuende.Application.IntegrationEvents;

public record UserResentVerificationEmailIntegrationEvent(string UserEmail,
                                                          string VerificationToken,
                                                          string UserId) : IntegrationEvent;