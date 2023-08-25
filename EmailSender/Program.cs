using MessageBus;
using MessageBus.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAPIServices(builder.Configuration);

var app = builder.Build();

await AddSubscriptions(app);

if (app.Environment.IsDevelopment()) { }

app.Run();

async Task AddSubscriptions(WebApplication app)
{
    var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();

    var subscribeTasks = new List<Task>
    {
        eventBus.Subscribe<RegistrationEvent>(),
        eventBus.Subscribe<UserResentVerificationEmailIntegrationEvent>(),
    };

    await Task.WhenAll(subscribeTasks);
}
