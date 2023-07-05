namespace IdentityDuende.Pages.EmailConfirmationInfo;

public record ViewModel(bool IsEmailConfirmed, string Email,string? ReturnUrl)
{
    public bool VerificationEmailResent { get; set; } = false;
}
