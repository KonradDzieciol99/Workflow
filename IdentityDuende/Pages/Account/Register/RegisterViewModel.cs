using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Account.Register
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        //[Required]
        //[Compare("Email", ErrorMessage = "The Email and Confirm Email fields do not match.")]
        //[NotEq("Email", ErrorMessage = "The password and confirmation password do not match.")]
        //[Compare(nameof(Email), ErrorMessage = "Passwords don't match.")]
        //public string RepeatPassword { get; set; }
        //public string ReturnUrl { get; set; }
        //public string RoleName { get; set; }
    }
}
