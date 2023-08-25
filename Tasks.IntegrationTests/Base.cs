using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Respawn;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Tasks.Infrastructure.DataAccess;

namespace Tasks.IntegrationTests;

[CollectionDefinition("Base")]
public class WebApplicationFactoryCollection : ICollectionFixture<Base>
{
    // Ta klasa nie ma żadnego kodu i nigdy nie jest tworzona bezpośrednio.
    // Jej celem jest po prostu być miejscem, gdzie można zastosować [CollectionDefinition] i wszystkie
    // interfejsy ICollectionFixture<>.
    //czyli potencjalnie kilka ICollectionFixture ?????
}

public class Base
{
    public readonly WebApplicationFactory<Program> _factory;
    public HttpClient _client;
    public readonly Respawner _checkpoint;
    public IConfiguration _configuration;

    public Base()
    {
        this._factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {

                builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

                    config.AddConfiguration(integrationConfig);
                });

                builder.ConfigureServices((context, services) =>
                {
                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    services.Remove(dbContextOptions);

                    var dbConnString = context.Configuration.GetConnectionString("TestDb") ?? throw new ArgumentNullException("dbConnString");
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(dbConnString,
                            builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                    services.AddAuthentication(opt =>
                    {
                        opt.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                        opt.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });

                });
            });

        this._client = _factory.CreateClient();
        this._configuration = _factory.Services.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException(nameof(IConfiguration)); ;

        this._checkpoint = Respawner.CreateAsync(_configuration.GetConnectionString("TestDb")!, new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
        }).GetAwaiter().GetResult();
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string UserId = "UserId";
    public const string UserEmail = "Email";

    public const string AuthenticationScheme = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock) { }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Test user"),
        };

        if (Context.Request.Headers.TryGetValue(UserId, out var userId))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId[0]));
        else
            throw new ArgumentNullException($"{nameof(UserId)}, Podaj Id użytkownika do Testów");

        if (Context.Request.Headers.TryGetValue(UserEmail, out var email))
            claims.Add(new Claim(ClaimTypes.Email, email[0]));
        else
            throw new ArgumentNullException($"{nameof(UserEmail)}, Podaj Email użytkownika do Testów");

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}