﻿using Projects.Infrastructure.DataAccess;
using TestsHelpers.Extensions;

namespace Services.IntegrationEvents.IntegrationTests;

public static class ProjectsBase
{
    public static WebApplicationFactory<Projects.Program> Init(
        string dBConnString,
        string RabbitMQConnString
    )
    {
        return new WebApplicationFactory<Projects.Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration(
                (context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["RabbitMQOptions:RabbitMQConnectionString"] = RabbitMQConnString,
                            ["RabbitMQOptions:Exchange"] = "workflow_event_bus",
                            ["RabbitMQOptions:Queue"] = "projects",
                        }
                    );
                }
            );

            builder.ConfigureServices(
                (context, services) =>
                {
                    services.Remove<DbContextOptions<ApplicationDbContext>>();

                    services.AddDbContext<ApplicationDbContext>(
                        options =>
                            options.UseSqlServer(
                                dBConnString,
                                builder =>
                                    builder.MigrationsAssembly(
                                        typeof(ApplicationDbContext).Assembly.FullName
                                    )
                            )
                    );

                    services
                        .AddAuthentication(opt =>
                        {
                            opt.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                            opt.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            TestAuthHandler.AuthenticationScheme,
                            options => { }
                        );
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy(
                            "ApiScope",
                            policy =>
                            {
                                policy.RequireAssertion(context => true);
                            }
                        );
                    });
                }
            );
        });
    }
}
