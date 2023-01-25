using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityServer.Pages.EmailConfirmation
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleInManager;
        private IdentityUser User;
        public string UserEmail;
        public IndexModel(UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                RoleManager<IdentityRole> roleInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleInManager = roleInManager;
        }
        
        public async Task<IActionResult> OnGet(string email,string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("~/", new { returnUrl });
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || user?.EmailConfirmed == true)
            {
                return RedirectToPage("/Account/Login/Index", new { returnUrl });
            }
            this.UserEmail = user.Email;
            this.User = user;
            
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {

            //tutaj call do wys³ania emaila 

            return Page();
        }
    }
}
