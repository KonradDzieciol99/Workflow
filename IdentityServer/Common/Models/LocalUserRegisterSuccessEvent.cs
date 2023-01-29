using Duende.IdentityServer.Events;

namespace IdentityServer.Common.Models
{
    public class LocalUserRegisterSuccessEvent : Event
    {
        public LocalUserRegisterSuccessEvent(string localUserEmail, string localUserActivateToken, string identityUserId)
            : base(EventCategories.Authentication,
                    "Local User Register",
                    EventTypes.Success,
                    EventIds.UserLoginSuccess//<--TODO
                    )
        {
            LocalUserEmail = localUserEmail;
            LocalUserActivateToken = localUserActivateToken;
            IdentityUserId = identityUserId;
        }

        public string LocalUserEmail { get; set; } 
        public string LocalUserActivateToken { get; set; } 
        public string IdentityUserId { get; set; } 
    }
}
