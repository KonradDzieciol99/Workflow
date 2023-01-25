using Duende.IdentityServer.Events;

namespace IdentityServer.Common.Models
{
    public class LocalUserRegisterSuccessEvent : Event
    {
        public LocalUserRegisterSuccessEvent(string Email)
            : base(EventCategories.Authentication,
                    "Local User Register",
                    EventTypes.Success,
                    EventIds.UserLoginSuccess//<--TODO
                    )
        {
            LocalUserEmail = Email;
        }

        public string LocalUserEmail { get; set; }
    }
}
