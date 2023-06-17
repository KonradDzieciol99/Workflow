using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityDuende.Pages.Device;

[SecurityHeaders]
[Authorize]
public class SuccessModel : PageModel
{
    public void OnGet()
    {
    }
}