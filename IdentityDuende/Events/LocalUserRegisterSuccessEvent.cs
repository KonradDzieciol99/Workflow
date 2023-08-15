using Duende.IdentityServer.Events;

namespace IdentityDuende.Events
{
    public class LocalUserRegisterSuccessEvent : Event
    {
        public LocalUserRegisterSuccessEvent(string localUserEmail, string localUserActivateToken, string identityUserId, string? identityUserPhotoUrl)
            : base(EventCategories.Authentication,
                    "Local User Register",
                    EventTypes.Success,
                    EventIds.UserLoginSuccess//<--TODO
                    )
        {
            LocalUserEmail = localUserEmail;
            LocalUserActivateToken = localUserActivateToken;
            IdentityUserId = identityUserId;
            IdentityUserPhotoUrl = identityUserPhotoUrl;
        }

        public string LocalUserEmail { get; set; }
        public string LocalUserActivateToken { get; set; }
        public string IdentityUserId { get; set; }
        public string? IdentityUserPhotoUrl { get; set; }
    }
}
