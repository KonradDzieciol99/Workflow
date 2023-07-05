using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityDuende.Entities;
using MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.Identity;

namespace IdentityDuende.Events
{
    public class IdentityEvents : IEventSink
    {
        private readonly IAzureServiceBusSender _azureServiceBusSender;

        //private readonly IMessageBus _messageBus;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityEvents(/*IMessageBus messageBus,*/IAzureServiceBusSender messageBus, UserManager<ApplicationUser> userManager)
        {
            _azureServiceBusSender = messageBus;
            _userManager = userManager;
        }
        public async Task PersistAsync(Event evt)
        {
            if (evt.Name == "Local User Register")
            {
                var localUserRegisterSuccessEvent = (LocalUserRegisterSuccessEvent)evt;
                //var registerEmailBusMessage = new RegisterEmailBusMessage() { Email = localUserRegisterSuccessEvent.LocalUserEmail, Token = localUserRegisterSuccessEvent.LocalUserActivateToken };
                var registerEmailBusMessage = new NewUserRegistrationEvent()
                {
                    //EventRecipient = new SimpleUser() { UserEmail = localUserRegisterSuccessEvent.LocalUserEmail, UserId = localUserRegisterSuccessEvent.IdentityUserId },
                    UserEmail = localUserRegisterSuccessEvent.LocalUserEmail,
                    Token = localUserRegisterSuccessEvent.LocalUserActivateToken,
                    UserId = localUserRegisterSuccessEvent.IdentityUserId
                };
                await _azureServiceBusSender.PublishMessage(registerEmailBusMessage);


                //var newUserRegisterCreateUser = new NewUserRegisterCreateUser() { Email = localUserRegisterSuccessEvent.LocalUserEmail, Id = localUserRegisterSuccessEvent.IdentityUserId };
                //await _messageBus.PublishMessage(newUserRegisterCreateUser, "new-user-register-create-user");
            }
            if (evt.Name == "External User Register")
            {
                var externalUserRegisterSuccessEvent = (ExternalUserRegisterSuccessEvent)evt;
                //TODO welcome Email
                //var newUserRegisterCreateUser = new NewUserRegisterCreateUser() { Email = externalUserRegisterSuccessEvent.ExternalUserEmail, Id = externalUserRegisterSuccessEvent.IdentityUserId };
                //await _messageBus.PublishMessage(newUserRegisterCreateUser, "new-user-register-create-user");
            }

            if(evt is UserResentVerificationEmailEvent @event)
            {
                var integrationEvent = new UserResentVerificationEmailIntegrationEvent(@event.User.Email!, @event.VerificationToken, @event.User.Id);
                await _azureServiceBusSender.PublishMessage(integrationEvent);
            }

            return;
        }
    }
}
