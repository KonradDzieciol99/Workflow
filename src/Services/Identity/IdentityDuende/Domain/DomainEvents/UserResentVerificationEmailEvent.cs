using Duende.IdentityServer.Events;
using IdentityDuende.Domain.Entities;

namespace IdentityDuende.Domain.DomainEvents;

public class UserResentVerificationEmailEvent : Event
{
    public UserResentVerificationEmailEvent(string verificationToken, ApplicationUser user) : base(EventCategories.Authentication, nameof(UserResentVerificationEmailEvent), EventTypes.Success, 111, null)
    {
        VerificationToken = verificationToken;
        User = user;
    }
    public string VerificationToken { get; set; }
    public ApplicationUser User { get; set; }
}