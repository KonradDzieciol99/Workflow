﻿using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityDuende.Application.IntegrationEvents;
using MessageBus;

namespace IdentityDuende.Domain.DomainEvents;

public class IdentityEvents : IEventSink
{
    private readonly IEventBusSender _azureServiceBusSender;

    public IdentityEvents(IEventBusSender messageBus)
    {
        _azureServiceBusSender = messageBus;
    }

    public async Task PersistAsync(Event evt)
    {
        if (evt.Name == "Local User Register")
        {
            var localUserRegisterSuccessEvent = (LocalUserRegisterSuccessEvent)evt;
            var registerEmailBusMessage = new RegistrationEvent(
                localUserRegisterSuccessEvent.LocalUserEmail,
                localUserRegisterSuccessEvent.LocalUserActivateToken,
                localUserRegisterSuccessEvent.IdentityUserId,
                null
            );

            await _azureServiceBusSender.PublishMessage(registerEmailBusMessage);
        }

        if (evt is UserResentVerificationEmailEvent @event)
        {
            var integrationEvent = new UserResentVerificationEmailIntegrationEvent(
                @event.User.Email!,
                @event.VerificationToken,
                @event.User.Id
            );
            await _azureServiceBusSender.PublishMessage(integrationEvent);
        }

        return;
    }
}
