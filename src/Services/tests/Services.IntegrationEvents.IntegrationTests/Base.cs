using Respawn;
using System.Text.RegularExpressions;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;

namespace Services.IntegrationEvents.IntegrationTests;

[CollectionDefinition("Base")]
public class WebApplicationFactoryCollection : ICollectionFixture<Base> { }

public class Base : IAsyncLifetime
{
    public Respawner? _checkpoint;
    public HttpClient _chatClient;
    public HttpClient _notificationClient;
    public HttpClient _projectsClient;
    public HttpClient _tasksClient;
    public readonly MsSqlContainer _msSqlContainer;
    public readonly RabbitMqContainer _rabbitMqContainer;

    public WebApplicationFactory<Chat.Program> _chatFactory;
    public WebApplicationFactory<Notification.Program> _notificationFactory;
    public WebApplicationFactory<Projects.Program> _projectsFactory;
    public WebApplicationFactory<Tasks.Program> _tasksFactory;

    public Base()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
        _rabbitMqContainer = new RabbitMqBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        _chatFactory = ChatBase.Init(
            ModifyConnectionString(_msSqlContainer.GetConnectionString(), "chat"),
            _rabbitMqContainer.GetConnectionString()
        );
        _notificationFactory = NotificationsBase.Init(
            ModifyConnectionString(_msSqlContainer.GetConnectionString(), "notification"),
            _rabbitMqContainer.GetConnectionString()
        );
        _projectsFactory = ProjectsBase.Init(
            ModifyConnectionString(_msSqlContainer.GetConnectionString(), "projects"),
            _rabbitMqContainer.GetConnectionString()
        );
        _tasksFactory = TasksBase.Init(
            ModifyConnectionString(_msSqlContainer.GetConnectionString(), "tasks"),
            _rabbitMqContainer.GetConnectionString()
        );

        _chatClient = _chatFactory.CreateClient();
        _notificationClient = _notificationFactory.CreateClient();
        _projectsClient = _projectsFactory.CreateClient();
        _tasksClient = _tasksFactory.CreateClient();

        _checkpoint = await Respawner.CreateAsync(
            _msSqlContainer.GetConnectionString(),
            new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            }
        );
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync().AsTask();
        await _rabbitMqContainer.DisposeAsync().AsTask();
    }

    private static string ModifyConnectionString(
        string originalConnectionString,
        string databaseName
    )
    {
        var dbMatch = Regex.Match(originalConnectionString, @"Database=\w+;");
        if (dbMatch.Success)
        {
            var newDbValue = $"Database={databaseName};";
            return originalConnectionString.Replace(dbMatch.Value, newDbValue);
        }
        return originalConnectionString;
    }
}
