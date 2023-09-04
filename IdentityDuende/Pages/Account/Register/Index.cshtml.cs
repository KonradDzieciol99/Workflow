using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityDuende.Domain.DomainEvents;
using IdentityDuende.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityDuende.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEventService _events;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;

    public RegisterViewModel RegisterViewModel { get; set; }

    public IndexModel(
        UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleInManager,
            IEventService events,
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityProviderStore identityProviderStore
          )
    {
        _roleManager = roleInManager;
        this._events = events;
        this._interaction = interaction;
        this._schemeProvider = schemeProvider;
        this._identityProviderStore = identityProviderStore;
        _userManager = userManager;
        _signInManager = signInManager;
    }


    [BindProperty]
    public RegisterInputModel Input { get; set; }


    public async Task<IActionResult> OnGet(string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return NotFound();

        await BuildModelAsync(returnUrl);
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser()
            {
                UserName = Input.Email,
                Email = Input.Email,
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _events.RaiseAsync(new LocalUserRegisterSuccessEvent(user.Email, token, user.Id, null));

                return RedirectToPage("/EmailConfirmationInfo/Index", new { email = user.Email, returnUrl = Input.ReturnUrl });
            }

            foreach (var item in result.Errors)
            {
                if (item.Code == "DuplicateUserName")
                    continue;

                ModelState.AddModelError(string.Empty, item.Description);
            }

        }
        await BuildModelAsync(Input.ReturnUrl);
        return Page();
    }

    private async Task BuildModelAsync(string returnUrl)
    {
        Input = new RegisterInputModel
        {
            ReturnUrl = returnUrl
        };

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new RegisterViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var dyanmicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new RegisterViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });
        providers.AddRange(dyanmicSchemes);

        RegisterViewModel = new RegisterViewModel
        {
            ExternalProviders = providers.ToArray(),
        };
    }
}