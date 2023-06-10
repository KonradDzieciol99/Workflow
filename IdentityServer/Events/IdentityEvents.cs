using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityServer.Common.Models;
using IdentityServer.Entities;
using MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Events
{
    public class IdentityEvents : IEventSink
    {
        private readonly IAzureServiceBusSender _messageBus;

        //private readonly IMessageBus _messageBus;
        private readonly UserManager<AppUser> _userManager;

        public IdentityEvents(/*IMessageBus messageBus,*/IAzureServiceBusSender messageBus, UserManager<AppUser> userManager)
        {
            _messageBus = messageBus;
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
                await _messageBus.PublishMessage(registerEmailBusMessage);


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
            return;
        }
    }
}
//var friendInvitationAcceptedEvent = new FriendInvitationAddedEvent()
//{

//    EventRecipient = new SimpleUser() { UserEmail = friendInvitation.InvitedUserEmail, UserId = friendInvitation.InvitedUserId },
//    EventSender = new SimpleUser() { UserEmail = userEmail, UserId = userId },
//    EventType = "FriendInvitationAddedEvent",
//    FriendInvitationDto = _mapper.Map<FriendInvitationDtoGlobal>(friendInvitation),
//    UserWhoInvited = new SimpleUser() { UserEmail = userEmail, UserId = userId },
//    InvitedUser = new SimpleUser() { UserEmail = friendInvitation.InvitedUserEmail, UserId = friendInvitation.InvitedUserId },
//};