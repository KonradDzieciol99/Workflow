using System.ComponentModel.DataAnnotations;

namespace IdentityDuende.Pages.Account.Register;

public class RegisterInputModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords must match.")]
    [Display(Name = "Repeat Password")]
    public string RepeatPassword { get; set; }

    public string ReturnUrl { get; set; }
}
