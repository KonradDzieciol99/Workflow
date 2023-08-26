using Duende.IdentityServer.Events;

namespace IdentityDuende.Events;

public class ExternalUserRegisterSuccessEvent : Event
{
    public ExternalUserRegisterSuccessEvent(string externalUserEmail, string identityUserId)
        : base(EventCategories.Authentication,
                "External User Register",
                EventTypes.Success,
                EventIds.UserLoginSuccess//<--TODO
                )
    {
        ExternalUserEmail = externalUserEmail;
        IdentityUserId = identityUserId;
    }

    public string ExternalUserEmail { get; set; }
    public string IdentityUserId { get; set; }
}
