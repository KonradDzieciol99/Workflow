using MessageBus.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureServiceBusSubscriber(opt =>
{
    var configuration = builder.Configuration;
    opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
    opt.QueueNameAndEventTypePair = new Dictionary<string, Type>()
        {
            {configuration.GetValue<string>("markChatMessageAsReadQueue"),typeof(MarkChatMessageAsReadEvent)},
            {configuration.GetValue<string>("newOnlineUserQueue"),typeof(NewOnlineUserEvent)},
            //{configuration.GetValue<string>("FriendInvitationAcceptedQueue"),typeof(FriendInvitationAcceptedEvent)},
        };
    opt.TopicNameWithSubscriptionNameAndEventTypePair = new Dictionary<Tuple<string, string>, Type>()
    {
        {Tuple.Create(configuration.GetValue<string>("AzureBusTopic"),configuration.GetValue<string>("AzureBusSubscription")),typeof(NewUserRegisterCreateUser)},
        {Tuple.Create(configuration.GetValue<string>("newOfflineUserTopic"),configuration.GetValue<string>("newOfflineUserTopicChatSub")),typeof(NewOfflineUserEvent)},
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
