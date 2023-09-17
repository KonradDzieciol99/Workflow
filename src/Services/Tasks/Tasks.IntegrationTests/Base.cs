using Bogus;
using MessageBus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Respawn;
using Testcontainers.MsSql;

namespace Tasks.IntegrationTests;


// można też uzywać IAsyncLifetime ale na to wygląda że odróżnai go od konstruktora i disposable to że jest asynchroniczny(i w sumie tyle)
// Jeżeli klasy dzieci są w tej samej kolekcji, czyli mają atrybut[Collection("Base")], to testy z tych klas będą uruchamiane sekwencyjnie, a nie równolegle.
[CollectionDefinition("Base")]
public class WebApplicationFactoryCollection : ICollectionFixture<Base>
{
    // Ta klasa nie ma żadnego kodu i nigdy nie jest tworzona bezpośrednio.
    // Jej celem jest po prostu być miejscem, gdzie można zastosować [CollectionDefinition] i wszystkie
    // interfejsy ICollectionFixture<>.
    //czyli potencjalnie kilka ICollectionFixture ?????
}

public class Base : IAsyncLifetime
{
    public readonly WebApplicationFactory<Program> _factory;
    public HttpClient? _client;
    public Respawner? _checkpoint;
    public readonly MsSqlContainer _msSqlContainer;

    public Base()
    {
        _msSqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                                            .Build();

        this._factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    var mockSender = new Mock<IEventBusSender>();
                    var mockConsumer = new Mock<IEventBusConsumer>();

                    mockSender.Setup(sender => sender.PublishMessage(It.IsAny<IntegrationEvent>())).Returns(Task.CompletedTask);
                    mockConsumer.Setup(consumer => consumer.Subscribe<IntegrationEvent>()).Returns(Task.CompletedTask);

                    services.AddSingleton<IEventBusSender>(mockSender.Object);
                    services.AddSingleton<IEventBusConsumer>(mockConsumer.Object);

                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    services.Remove(dbContextOptions);

                    var dbConnString = _msSqlContainer.GetConnectionString() ?? throw new ArgumentNullException("dbConnString");
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(dbConnString,
                            builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                    services.AddAuthentication(opt =>
                    {
                        opt.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                        opt.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy("ApiScope", policy =>
                        {
                            policy.RequireAssertion(context => true);
                        });
                    });
                });
            });
    }


    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        _client = _factory.CreateClient();

        _checkpoint = await Respawner.CreateAsync(_msSqlContainer.GetConnectionString(), new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
        });
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync().AsTask();
    }
}