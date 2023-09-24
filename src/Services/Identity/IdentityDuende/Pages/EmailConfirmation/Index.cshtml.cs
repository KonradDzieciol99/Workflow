using IdentityDuende.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityDuende.Pages.EmailConfirmation;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        this._userManager = userManager;
    }

    public ViewModel View { get; set; }

    public async Task<IActionResult> OnGet(string token, string email)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                if (user.EmailConfirmed)
                    return RedirectToPage(
                        "/EmailConfirmationInfo",
                        new { email = user.Email, returnUrl = View.ReturnUrl }
                    );

                var resoult = await _userManager.ConfirmEmailAsync(user, token);
                if (resoult.Succeeded)
                {
                    View = new ViewModel(success: true);

                    return Page();
                }
            }
        }

        return NotFound();
    }
}
