using Microsoft.AspNetCore.Mvc.Testing;
using Respawn;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Bogus;
using Moq;
using API.Aggregator;
using TestsHelpers;
using WireMock.Server;
using Microsoft.Extensions.Configuration;

namespace API.Aggreggator.IntegrationTests;

[CollectionDefinition("Base")]
public class WebApplicationFactoryCollection : ICollectionFixture<Base>
{

}
public class Base : IAsyncLifetime
{
    public readonly WebApplicationFactory<Program> _factory;
    public HttpClient? _client;
    public Respawner? _checkpoint;
    public readonly MsSqlContainer _msSqlContainer;
    public readonly WireMockServer _mockServer;

    public Base()
    {
        _msSqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                                            .Build();
        _mockServer = WireMockServer.Start();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["urls:internal:tasks"] = _mockServer.Urls[0],
                    ["urls:internal:notification"] = _mockServer.Urls[0],
                    ["urls:internal:chat"] = _mockServer.Urls[0],
                    ["urls:internal:projects"] = _mockServer.Urls[0],
                    ["urls:internal:identity"] = _mockServer.Urls[0],
                    ["isTest"] = "true"
                });
            });

            builder.ConfigureServices((context, services) =>
            {
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