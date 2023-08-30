using IdentityDuende.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityDuende.Infrastructure.DataAccess;

public class SeedData
{
    private readonly ILogger<SeedData> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SeedData(ILogger<SeedData> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager/*,RoleManager<AppRole> roleManager*/ )
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
                await _context.Database.MigrateAsync();

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
        var alice = await _userManager.FindByEmailAsync("AliceSmith@email.com");
        if (alice is not null)
            _logger.LogDebug("alice already exists");
        else
        {
            alice = new ApplicationUser
            {
                UserName = "AliceSmith@email.com",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true,
                PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
            };

            var result = _userManager.CreateAsync(alice, "Pass123$").Result;
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);


            _logger.LogDebug("alice created");
        }


        var bob = await _userManager.FindByEmailAsync("BobSmith@email.com");
        if (bob is not null)
            _logger.LogDebug("bob already exists");
        else
        {
            bob = new ApplicationUser
            {
                UserName = "BobSmith@email.com",
                Email = "BobSmith@email.com",
                EmailConfirmed = true,
                PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/bobPhoto.png"
            };
            var result = _userManager.CreateAsync(bob, "Pass123$").Result;
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            _logger.LogDebug("bob created");
        }

    }
}
