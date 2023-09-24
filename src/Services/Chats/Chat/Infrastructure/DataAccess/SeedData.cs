using Bogus;
using Chat.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.DataAccess;

public class SeedData
{
    private readonly ILogger<SeedData> _logger;
    private readonly ApplicationDbContext _context;

    public SeedData(ILogger<SeedData> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
        var friendRequestsForAlice = new Faker<FriendRequest>()
            .StrictMode(false)
            .RuleFor(f => f.InviterUserId, f => "50")
            .RuleFor(f => f.InviterUserEmail, f => "AliceSmith@email.com")
            .RuleFor(
                f => f.InviterPhotoUrl,
                f => "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"
            )
            .RuleFor(f => f.InvitedUserId, f => id++.ToString())
            .RuleFor(u => u.InvitedUserEmail, f => f.Internet.Email())
            .RuleFor(u => u.InvitedPhotoUrl, f => f.Image.PicsumUrl(600, 600))
            .FinishWith(
                (f, u) =>
                {
                    u.AcceptRequest(u.InvitedUserId);
                }
            )
            .UseSeed(1111)
            .Generate(50);

        var friendRequest = await _context.FindAsync<FriendRequest>(
            friendRequestsForAlice[0].InviterUserId,
            friendRequestsForAlice[0].InvitedUserId
        );
        if (friendRequest is not null)
        {
            _logger.LogDebug($"{nameof(friendRequest)} already exists", friendRequest);
            return;
        }

        await _context.FriendRequests.AddRangeAsync(friendRequestsForAlice);

        _ = await _context.SaveChangesAsync();

        _logger.LogDebug("Seeding completed.");
    }
}
