using EmailEmitter.IntegrationEvents;
using HealthChecks.UI.Client;
using MessageBus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;

namespace EmailEmitter;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddWebAPIServices(builder.Configuration);
        var app = builder.Build();

        if (builder.Configuration.GetValue<bool>("SendGrid:enabled"))
            await AddSubscriptions(app);

        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        if (app.Environment.IsDevelopment()) { }

        app.Run();
    }
    private static async Task AddSubscriptions(WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBusConsumer>();

        var subscribeTasks = new List<Task>
        {
            eventBus.Subscribe<RegistrationEvent>(),
            eventBus.Subscribe<UserResentVerificationEmailIntegrationEvent>(),
        };

        await Task.WhenAll(subscribeTasks);
    }
}