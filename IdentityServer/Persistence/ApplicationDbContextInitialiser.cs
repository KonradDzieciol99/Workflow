using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
using System.Security.Claims;
using IdentityServer.Events;
using Duende.IdentityServer.Services;
using Mango.MessageBus;
using MessageBus;
using MessageBus.Events;
using IdentityServer.Entities;

namespace IdentityServer.Persistence
{
    public class ApplicationDbContextInitialiser
    {
       // private readonly IMessageBus _messageBus;
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEventService _events;
        //private readonly IMessageBus _messageBus;
        //private readonly RoleManager<AppRole> _roleManager;

        public ApplicationDbContextInitialiser(/*IMessageBus messageBus,*/ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<AppUser> userManager/*,RoleManager<AppRole> roleManager*/ )
        {
            //this._messageBus = messageBus;
            //this._events = events;
            _logger = logger;
            _context = context;
            _userManager = userManager;
           // _roleManager = roleManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            var alice = _userManager.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new AppUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = _userManager.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }


                result = _userManager.AddClaimsAsync(alice, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                    new Claim(JwtClaimTypes.Picture, "https://www.iesabroad.org/sites/default/files/styles/blog_card/public/2022-07/09/fullsizeoutput_9331-featured-cropped.jpg?itok=IHM5Tlld"),

                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                //var newUserRegisterCreateUser = new NewUserRegisterCreateUser() { Email = alice.Email, Id = alice.Id };
                //await _messageBus.PublishMessage(newUserRegisterCreateUser, "new-user-register-create-user");
                //await _events.RaiseAsync(new ExternalUserRegisterSuccessEvent(alice.Email, alice.Id));

                // Log.Debug("alice created");
            }
            else
            {
                //Log.Debug("alice already exists");
            }

            var bob = _userManager.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new AppUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = _userManager.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = _userManager.AddClaimsAsync(bob, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    new Claim("location", "somewhere"),
                    new Claim(JwtClaimTypes.Picture, "https://tvn24.pl/tvnmeteo/najnowsze/cdn-zdjecie-x921ur-filary-stworzenia-w-obiektywie-kosmicznego-teleskopu-jamesa-webba-6171706/alternates/LANDSCAPE_840"),

                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                //var newUserRegisterCreateUser = new NewUserRegisterCreateUser() { Email = bob.Email, Id = bob.Id };
                //await _messageBus.PublishMessage(newUserRegisterCreateUser, "new-user-register-create-user");
                //await _events.RaiseAsync(new ExternalUserRegisterSuccessEvent(bob.Email, bob.Id));
                //Log.Debug("bob created");
            }
            else
            {
                //Log.Debug("bob already exists");
            }
        }
    }
}
