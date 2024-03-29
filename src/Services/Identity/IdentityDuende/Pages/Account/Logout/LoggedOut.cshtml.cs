using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityDuende.Pages.Account.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class LoggedOut : PageModel
{
    private readonly IIdentityServerInteractionService _interactionService;

    public LoggedOutViewModel View { get; set; }

    public LoggedOut(IIdentityServerInteractionService interactionService)
    {
        _interactionService =
            interactionService ?? throw new ArgumentNullException(nameof(interactionService));
    }

    public async Task<IActionResult> OnGet(string logoutId)
    {
        // get context information (client name, post logout redirect URI and iframe for federated signout)
        var logout = await _interactionService.GetLogoutContextAsync(logoutId);

        View = new LoggedOutViewModel
        {
            AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
            ClientName = string.IsNullOrEmpty(logout?.ClientName)
                ? logout?.ClientId
                : logout?.ClientName,
            SignOutIframeUrl = logout?.SignOutIFrameUrl
        };

        if (
            LogoutOptions.AutomaticRedirectAfterSignOut
            && !string.IsNullOrWhiteSpace(logout?.PostLogoutRedirectUri)
        )
            return Redirect(logout?.PostLogoutRedirectUri);

        return Page();
    }
}
