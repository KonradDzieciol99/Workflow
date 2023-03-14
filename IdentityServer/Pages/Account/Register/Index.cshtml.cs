using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityServer.Common.Models;
using IdentityServer.Entities;
using IdentityServer.Events;
using IdentityServerHost.Pages.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace IdentityServer.Pages.Account.Register
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEventService _events;

        public IndexModel(
            UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                RoleManager<IdentityRole> roleInManager,
                IEventService events
              )
        {
            _roleManager = roleInManager;
            this._events = events;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [BindProperty]
        public RegisterViewModel Input { get; set; }


        public async Task<IActionResult> OnGet()
        {
            //string returnUrl
            //List<string> roles = new()
            //{
            //    SD.Admin,
            //    SD.Customer
            //};
            //ViewData["roles_message"] = roles;
            //Input = new RegisterViewModel
            //{
            //    ReturnUrl = returnUrl
            //};
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            //string returnUrl
            if (ModelState.IsValid)
            {
                var user = new AppUser()
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    //if (!_roleManager.RoleExistsAsync(Input.RoleName).GetAwaiter().GetResult())
                    //{
                    //    var userRole = new IdentityRole
                    //    {
                    //        Name = Input.RoleName,
                    //        NormalizedName = Input.RoleName,

                    //    };
                    //    await _roleManager.CreateAsync(userRole);
                    //}
                    //await _userManager.AddToRoleAsync(user, Input.RoleName);


                    await _userManager.AddClaimsAsync(user, new Claim[] {
                        new Claim(JwtClaimTypes.Email,Input.Email),
                    });
                    var identityUser = await _userManager.FindByEmailAsync(user.Email);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                    await _events.RaiseAsync(new LocalUserRegisterSuccessEvent(user.Email,token,user.Id));

                    return RedirectToPage("/EmailConfirmationInfo/Index", new { user.Email });

                    //var loginresult = await _signInManager.PasswordSignInAsync(
                    //    Input.Email, Input.Password, false, lockoutOnFailure: true);

                    //if (loginresult.Succeeded)
                    //{
                        //return RedirectToPage("/EmailConfirmation/Index", new { user.Email });
                        //if (Url.IsLocalUrl(Input.ReturnUrl))
                        //{
                        //    return Redirect(Input.ReturnUrl);
                        //}
                        //else if (string.IsNullOrEmpty(Input.ReturnUrl))
                        //{
                        //    return Redirect("~/");
                        //}
                        //else
                        //{
                        //    throw new Exception("invalid return URL");
                        //}
                    //}
                }
                
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                
            }
            return Page();
        }
    }
}
