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

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
        }

        //[FromQuery(Name = "email")]
        //public string FooBar { get; set; }
        public async Task<IActionResult> OnGet(string token,string email)
        {
            var data = Request.Query["foo-bar"];
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    if (user.EmailConfirmed)
                    {
                        return Redirect("https://localhost:4200");
                    }

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
