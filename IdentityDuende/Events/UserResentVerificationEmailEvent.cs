using Duende.IdentityServer.Events;
using IdentityDuende.Entities;

namespace IdentityDuende.Events;

public class UserResentVerificationEmailEvent : Event
{
    public UserResentVerificationEmailEvent(string verificationToken, ApplicationUser user) : base(EventCategories.Authentication, nameof(UserResentVerificationEmailEvent), EventTypes.Success, 111, null)
    {
        VerificationToken=verificationToken;
        User = user;
    }
    public string VerificationToken { get; set; }
    public ApplicationUser User { get; set; }
}