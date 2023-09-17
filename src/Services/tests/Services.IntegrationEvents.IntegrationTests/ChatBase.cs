using Bogus;
using Chat.Domain.Entity;
using Chat.Infrastructure.DataAccess;

namespace Services.IntegrationEvents.IntegrationTests;

public static class ChatBase
{
    public static WebApplicationFactory<Chat.Program> Init(string dBConnString,string RabbitMQConnString)
    {
        return new WebApplicationFactory<Chat.Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RabbitMQConsumerOptions:RabbitMQConnectionString"] = RabbitMQConnString
                });
            });

            builder.ConfigureServices((context, services) =>
            {

                var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(dbContextOptions);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(dBConnString,
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
    public static List<FriendRequest> GetFakeFriendRequests(int amount = 1, bool confirmed = false, string? staticInviterUserId = null, string? staticInviterUserEmail = null, string? staticInvitedUserId = null, string? staticInvitedUserEmail = null)
    {

        return new Faker<FriendRequest>()
                             .StrictMode(false)
                             .CustomInstantiator(f =>
                                new FriendRequest(staticInviterUserId is null ? Guid.NewGuid().ToString() : staticInviterUserId,
                                                  staticInviterUserEmail is null ? f.Internet.Email() : staticInviterUserEmail,
                                                  null,
                                                  staticInvitedUserId is null ? Guid.NewGuid().ToString() : staticInvitedUserId,
                                                  staticInvitedUserEmail is null ? f.Internet.Email() : staticInvitedUserEmail,
                                                  null)
                             )
                             .FinishWith((f, friendRequest) =>
                             {
                                 if (confirmed)
                                     friendRequest.AcceptRequest(friendRequest.InvitedUserId);
                             })
                             .Generate(amount);
    }
}