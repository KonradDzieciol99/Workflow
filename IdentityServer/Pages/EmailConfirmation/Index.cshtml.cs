using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityServer.Pages.EmailConfirmation
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<IActionResult> OnGet(string token,string email)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    var resoult =await _userManager.ConfirmEmailAsync(user, token);
                    if (resoult.Succeeded)
                    {
                        return Page();
                    }
                }
            }

            return Redirect("https://localhost:4200");
        }
    }
}
