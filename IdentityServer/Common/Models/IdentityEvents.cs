using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Mango.MessageBus;
using MessageBus;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Common.Models
{
    public class IdentityEvents : IEventSink
    {
        private readonly IMessageBus _messageBus;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityEvents(IMessageBus messageBus, UserManager<IdentityUser> userManager)
        {
            this._messageBus = messageBus;
            this._userManager = userManager;
        }
        public async Task PersistAsync(Event evt)
        {
            if (evt.Name == "Local User Register")
            {
                var localUserRegisterSuccessEvent = (LocalUserRegisterSuccessEvent)evt;
                var registerEmailBusMessage =  new RegisterEmailBusMessage() { Email = localUserRegisterSuccessEvent.LocalUserEmail,Token = localUserRegisterSuccessEvent.LocalUserActivateToken };
                await _messageBus.PublishMessage(registerEmailBusMessage, "newuserregister");
                var newUserRegisterCreateUser = new NewUserRegisterCreateUser() { Email = localUserRegisterSuccessEvent.LocalUserEmail,Id= localUserRegisterSuccessEvent.IdentityUserId };
                await _messageBus.PublishMessage(newUserRegisterCreateUser, "new-user-register-create-user");

            }
            return;
        }
    }
}
