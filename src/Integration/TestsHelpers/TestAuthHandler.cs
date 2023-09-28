using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TestsHelpers;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string UserId = "UserId";
    public const string UserEmail = "Email";
    public const string AuthenticationScheme = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    )
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Test user"), };

        if (Context.Request.Headers.TryGetValue(UserId, out var userId))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId[0]));
        else
            throw new InvalidOperationException($"{nameof(UserId)}, Podaj Id użytkownika do Testów");

        if (Context.Request.Headers.TryGetValue(UserEmail, out var email))
            claims.Add(new Claim(ClaimTypes.Email, email[0]));
        else
            throw new InvalidOperationException(
                $"{nameof(UserEmail)}, Podaj Email użytkownika do Testów"
            );

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
