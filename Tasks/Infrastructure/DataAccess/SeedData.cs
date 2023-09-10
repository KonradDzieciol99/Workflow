using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;

namespace Tasks.Infrastructure.DataAccess;

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

        var member = new ProjectMember("1","1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", ProjectMemberType.Leader, InvitationStatus.Accepted,"1");

        var now = DateTime.UtcNow;

        var faker = new Faker<AppTask>()
           .StrictMode(false)
           .CustomInstantiator(f => new AppTask(
               f.Lorem.Word(),
               f.Lorem.Sentences(),
               member.ProjectId,
               null,
               f.PickRandom<Priority>(),
               f.PickRandom<State>(),
               f.Date.Between(now.AddDays(7),now.AddMonths(4)),
               f.Date.Between(now, now.AddDays(7)),
               member.Id
               )
           );

        var tasks = faker.Generate(140);

        var projectsCount = await _context.AppTasks.CountAsync();
        if (projectsCount >= 140)
        {
            _logger.LogDebug($"{nameof(AppTask)} seed already exists");
            return;
        }

        _context.ProjectMembers.Add(member);

        await _context.SaveChangesAsync();
        
        _context.AppTasks.AddRange(tasks);

        await _context.SaveChangesAsync();

        _logger.LogDebug("Seeding completed.");
    }
}
