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

        List<FriendRequest> friendRequestsForAlice = new()
        {
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "0", "BobSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/bobPhoto.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "2", "JamesSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/JamesSmith.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "3", "EmilyJohnson@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "4", "JohnWilliams@email.com", "https://1workflowstorage.blob.core.windows.net/photos/JohnWilliams.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "5", "OliviaJones@email.com", "https://1workflowstorage.blob.core.windows.net/photos/OliviaJones.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "6", "MichaelBrown@email.com", "https://1workflowstorage.blob.core.windows.net/photos/MichaelBrown.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "7", "SophiaDavis@email.com", "https://1workflowstorage.blob.core.windows.net/photos/SophiaDavis.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "8", "WilliamMiller@email.com", "https://1workflowstorage.blob.core.windows.net/photos/WilliamMille.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "9", "AvaWilson1@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AvaWilson1.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "10", "AvaWilson2@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AvaWilson2.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "11", "AvaWilson3@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AvaWilson3.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "12", "DavidMoore@email.com", "https://1workflowstorage.blob.core.windows.net/photos/DavidMoore.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "13", "IsabellaTaylor@email.com", "https://1workflowstorage.blob.core.windows.net/photos/IsabellaTaylor.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "14", "ChristopherWhite@email.com", "https://1workflowstorage.blob.core.windows.net/photos/ChristopherWhite.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "15", "MiaHarris@email.com", "https://1workflowstorage.blob.core.windows.net/photos/MiaHarris.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "16", "MatthewMartin@email.com", "https://1workflowstorage.blob.core.windows.net/photos/MatthewMartin.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "17", "AmeliaThompson@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AmeliaThompson.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "18", "DavidMoore@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "19", "IsabellaTaylor@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "20", "ChristopherWhite@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "21", "MiaHarris@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "22", "MatthewMartin@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "23", "AmeliaThompson@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
            new FriendRequest("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", "24", "AndrewGarcia@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png"),
        };

        foreach (var item in friendRequestsForAlice)
            item.AcceptRequest(item.InvitedUserId);
        

        var friendRequest = await _context.FindAsync<FriendRequest>(friendRequestsForAlice[0].InviterUserId, friendRequestsForAlice[0].InvitedUserId);
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
