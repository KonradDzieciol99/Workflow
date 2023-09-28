using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityDuende.Domain.DomainEvents;
using IdentityDuende.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace IdentityDuende.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEventService _events;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public RegisterViewModel RegisterViewModel { get; set; }

    public IndexModel(
        IIdentityServerInteractionService interaction,
        UserManager<ApplicationUser> userManager,
        IEventService events,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IConfiguration configuration,
        SignInManager<ApplicationUser> signInManager
    )
    {
        this._events = events ?? throw new ArgumentNullException(nameof(events));
        this._schemeProvider =
            schemeProvider ?? throw new ArgumentNullException(nameof(schemeProvider));
        this._identityProviderStore =
            identityProviderStore ?? throw new ArgumentNullException(nameof(identityProviderStore));
        this._configuration =
            configuration ?? throw new ArgumentNullException(nameof(configuration));
        this._signInManager =
            signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        this._interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
            var user = new ApplicationUser() { UserName = Input.Email, Email = Input.Email, };

            if (!_configuration.GetValue<bool>("EmailEmiterEnabled"))
                user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!_configuration.GetValue<bool>("EmailEmiterEnabled"))
            {
                _ = await _signInManager.PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    false,
                    lockoutOnFailure: true
                );
                var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
                await _events.RaiseAsync(
                    new UserLoginSuccessEvent(
                        user.UserName,
                        user.Id,
                        user.UserName,
                        clientId: context?.Client.ClientId
                    )
                );
                return Redirect(Input.ReturnUrl);
            }

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _events.RaiseAsync(
                    new LocalUserRegisterSuccessEvent(user.Email, token, user.Id, null)
                );

                return RedirectToPage(
                    "/EmailConfirmationInfo/Index",
                    new { email = user.Email, returnUrl = Input.ReturnUrl }
                );
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
        Input = new RegisterInputModel { ReturnUrl = returnUrl };

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(
                x =>
                    new RegisterViewModel.ExternalProvider
                    {
                        DisplayName = x.DisplayName ?? x.Name,
                        AuthenticationScheme = x.Name
                    }
            )
            .ToList();

        var dyanmicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(
                x =>
                    new RegisterViewModel.ExternalProvider
                    {
                        AuthenticationScheme = x.Scheme,
                        DisplayName = x.DisplayName
                    }
            );
        providers.AddRange(dyanmicSchemes);

        RegisterViewModel = new RegisterViewModel { ExternalProviders = providers.ToArray(), };
    }
}
