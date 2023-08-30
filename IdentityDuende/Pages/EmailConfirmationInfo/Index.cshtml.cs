using Duende.IdentityServer.Services;
using IdentityDuende.Entities;
using IdentityDuende.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityDuende.Pages.EmailConfirmationInfo;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleInManager;
    private readonly IEventService _events;
    public ViewModel View { get; set; }

    public IndexModel(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleInManager,
            IEventService events)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleInManager = roleInManager;
        this._events = events;
    }

    public async Task<IActionResult> OnGet(string email, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrWhiteSpace(email))
                return NotFound();

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return NotFound();

            View = new ViewModel(user.EmailConfirmed, email, returnUrl);

            return Page();
        }
        return NotFound();
    }
    public async Task<IActionResult> OnPost()
    {
        var email = Request.Query["email"];
        var returnUrl = Request.Query["returnUrl"];

        var user = await _userManager.FindByEmailAsync(email);

        if (user.EmailConfirmed)
        {
            View = new ViewModel(user.EmailConfirmed, email, returnUrl);
            return Page();
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _events.RaiseAsync(new UserResentVerificationEmailEvent(token, user));

        View = new ViewModel(user.EmailConfirmed, email, returnUrl);
        View.VerificationEmailResent = true;

        return Page();
    }
}
