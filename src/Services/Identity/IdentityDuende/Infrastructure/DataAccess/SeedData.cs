using Bogus;
using IdentityDuende.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityDuende.Infrastructure.DataAccess;

public class SeedData
{
    private readonly ILogger<SeedData> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SeedData(
        ILogger<SeedData> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager /*,RoleManager<AppRole> roleManager*/
    )
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
        var id = 0;
        var users = new Faker<ApplicationUser>()
            .StrictMode(false)
            .RuleFor(f => f.Id, f => id++.ToString())
            .RuleFor(f => f.UserName, f => f.Internet.Email())
            .RuleFor(f => f.Email, (f, u) => u.UserName)
            .RuleFor(f => f.PictureUrl, f => f.Image.PicsumUrl(600, 600))
            .RuleFor(u => u.EmailConfirmed, f => true)
            .UseSeed(1111)
            .Generate(50);

        users.Add(
            new ApplicationUser
            {
                UserName = "AliceSmith@email.com",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true,
                PictureUrl = "http://127.0.0.1:10000/devstoreaccount1/photos/AlicePicture.png",
                Id = "50"
            }
        );
        users.Add(
            new ApplicationUser
            {
                UserName = "BobSmith@email.com",
                Email = "BobSmith@email.com",
                EmailConfirmed = true,
                PictureUrl = "http://127.0.0.1:10000/devstoreaccount1/photos/bobPhoto.png",
                Id = "51"
            }
        );

        var result = await _userManager.FindByEmailAsync(users[0].Email);
        if (result is not null)
            return;

        foreach (var item in users)
        {
            var createResult = await _userManager.CreateAsync(item, "Pass123$");
            if (!createResult.Succeeded)
                throw new Exception(createResult.Errors.First().Description);
        }

        _logger.LogDebug("Seeding completed.");
    }
}
