using Tasks.Infrastructure.DataAccess;

namespace Services.IntegrationEvents.IntegrationTests;
public static class TasksBase
{
    public static WebApplicationFactory<Tasks.Program> Init(string dBConnString, string RabbitMQConnString)
    {
        return new WebApplicationFactory<Tasks.Program>().WithWebHostBuilder(builder =>
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
}
