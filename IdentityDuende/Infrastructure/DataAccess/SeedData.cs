using IdentityDuende.Domain.Entities;
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

        List<ApplicationUser> users = new()
        {
           new ApplicationUser
           {
               UserName = "BobSmith@email.com",
               Email = "BobSmith@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/bobPhoto.png"
           },
           new ApplicationUser
           {
               UserName = "AliceSmith@email.com",
               Email = "AliceSmith@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "JamesSmith@email.com",
               Email = "JamesSmith@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/JamesSmith.png"
           },
           new ApplicationUser
           {
               UserName = "EmilyJohnson@email.com",
               Email = "EmilyJohnson@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "JohnWilliams@email.com",
               Email = "JohnWilliams@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/JohnWilliams.png"
           },
           new ApplicationUser
           {
               UserName = "OliviaJones@email.com",
               Email = "OliviaJones@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/OliviaJones.png"
           },
           new ApplicationUser
           {
               UserName = "MichaelBrown@email.com",
               Email = "MichaelBrown@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/MichaelBrown.png"
           },
           new ApplicationUser
           {
               UserName = "SophiaDavis@email.com",
               Email = "SophiaDavis@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/SophiaDavis.png"
           },
           new ApplicationUser
           {
               UserName = "WilliamMiller@email.com",
               Email = "WilliamMiller@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/WilliamMille.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email.com",
               Email = "AvaWilson@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email2.com",
               Email = "AvaWilson@email2.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email3.com",
               Email = "AvaWilson@email3.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email4.com",
               Email = "AvaWilson@email4.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email5.com",
               Email = "AvaWilson@email5.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email6.com",
               Email = "AvaWilson@email6.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email7.com",
               Email = "AvaWilson@email7.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email8.com",
               Email = "AvaWilson@email8.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AvaWilson@email9.com",
               Email = "AvaWilson@email9.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "DavidMoore@email.com",
               Email = "DavidMoore@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "IsabellaTaylor@email.com",
               Email = "IsabellaTaylor@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "ChristopherWhite@email.com",
               Email = "ChristopherWhite@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "MiaHarris@email.com",
               Email = "MiaHarris@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "MatthewMartin@email.com",
               Email = "MatthewMartin@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AmeliaThompson@email.com",
               Email = "AmeliaThompson@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
           new ApplicationUser
           {
               UserName = "AndrewGarcia@email.com",
               Email = "AndrewGarcia@email.com",
               EmailConfirmed = true,
               PictureUrl = "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
           },
        };

        int idCounter = 0;
        foreach (var item in users)
        {
            var user = await _userManager.FindByEmailAsync(item.Email);
            if (user is not null)
            {
                _logger.LogDebug($"{item.Email} already exists");
                continue;
            }

            item.Id = idCounter.ToString();
            idCounter++;
            var result =  await _userManager.CreateAsync(item, "Pass123$");
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            _logger.LogDebug($"{item.Email} created", item);
        }

        _logger.LogDebug("Seeding completed.");

    }
}
