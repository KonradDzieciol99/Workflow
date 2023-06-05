using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Tasks.Infrastructure.DataAccess;

namespace Tasks.IntegrationTests;

[CollectionDefinition("Base")]
public class WebApplicationFactoryCollection: ICollectionFixture<Base>
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

    public Base()
    {
        this._factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    services.Remove(dbContextOptions);
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testDb"));
                    services.AddAuthentication(opt =>
                    {
                        opt.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                        opt.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => {});

                });

            });

        this._client = _factory.CreateClient();
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
        ISystemClock clock) : base(options, logger, encoder, clock){}
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