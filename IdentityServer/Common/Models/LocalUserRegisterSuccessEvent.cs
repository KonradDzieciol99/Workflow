using Duende.IdentityServer.Events;

namespace IdentityServer.Common.Models
{
    public class LocalUserRegisterSuccessEvent : Event
    {
        public LocalUserRegisterSuccessEvent(string email, string token)
            : base(EventCategories.Authentication,
                    "Local User Register",
                    EventTypes.Success,
                    EventIds.UserLoginSuccess//<--TODO
                    )
        {
            LocalUserEmail = email;
            Token = token;
        }

        public string LocalUserEmail { get; set; } 
        public string Token { get; set; } 
    }
}
