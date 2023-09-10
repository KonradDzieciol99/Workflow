using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Projects.Application.Common.Interfaces;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Domain.Common.Enums;
using Projects.Infrastructure.Services;
using System.Reflection;

namespace Projects.Infrastructure.DataAccess;

public class SeedData
{
    private readonly ILogger<SeedData> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventService _integrationEventService;

    public SeedData(ILogger<SeedData> logger, ApplicationDbContext context,IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
    {
        _logger = logger;
        _context = context;
        this._unitOfWork = unitOfWork;
        this._integrationEventService = integrationEventService;
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

        var occupations = new string[] { "gardener", "teacher", "writer", "programmer" };

        var photosList = new string[] { "avocadoChair.png", "bussinesStuff1.png", "bussinesStuff2.png",
        "bussinesStuff3.png","bussinesStuff4.png","bussinesStuff5.png","bussinesStuff6.png","dogo.png","gears.png","handPalm.png"};


        var leader = new ProjectMember("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", ProjectMemberType.Leader, InvitationStatus.Accepted);
        
        var leaderIdInfo = leader.GetType().GetProperty("Id");
        leaderIdInfo.SetValue(leader, "1", null);

        var mainProject = new Project("Testable", "https://1workflowstorage.blob.core.windows.net/projectsicons/dogo.png", leader);

        var idProperty = mainProject.GetType().GetProperty("Id");
        idProperty.SetValue(mainProject, "1",null);
        
        var faker = new Faker<Project>()
           .StrictMode(false)
           .CustomInstantiator(f => new Project(
               f.Commerce.ProductName(),
               $"https://1workflowstorage.blob.core.windows.net/projectsicons/{f.PickRandom(photosList)}",
               new("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", ProjectMemberType.Leader, InvitationStatus.Accepted)
               )
           //(Activator.CreateInstance(typeof(Project), nonPublic: true) as Project)
           );
           //.RuleFor(u => u.Name, f => f.Commerce.ProductName())
           //.RuleFor(u => u.IconUrl, f => $"https://1workflowstorage.blob.core.windows.net/projectsicons/{f.PickRandom(photosList)}")
           //.RuleFor(u => u.ProjectMembers, f => new List<ProjectMember>()
           //{
           //     new("1", "AliceSmith@email.com", "https://1workflowstorage.blob.core.windows.net/photos/AlicePicture.png", ProjectMemberType.Leader, InvitationStatus.Accepted),
           //})
           //.RuleFor(u => u.Id, f => Guid.NewGuid().ToString());

        var projects = faker.Generate(110);
        projects.Add(mainProject);

        var projectsCount = await _context.Projects.CountAsync();
        if (projectsCount >=100)
        {
            _logger.LogDebug($"{nameof(Project)} seed already exists");
            return;
        }

        await _context.Projects.AddRangeAsync(projects);

        _ = await _unitOfWork.Complete();

        await _integrationEventService.PublishEventsThroughEventBusAsync();

        _logger.LogDebug("Seeding completed.");
    }
}
