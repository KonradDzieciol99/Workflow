using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Socjal.API;
using Socjal.API.Common.MapperProfiles;
using Socjal.API.MessageBus;
using Socjal.API.Persistence;
using Socjal.API.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
}).AddStackExchangeRedis(builder.Configuration.GetConnectionString("Redis"), options =>
{
    options.Configuration.ChannelPrefix = "Socjal.API";
});

var RedisOptions = new ConfigurationOptions()
{
    EndPoints = { { builder.Configuration.GetConnectionString("Redis") } },
    IncludeDetailInExceptions = true,
};

builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(RedisOptions)
);///////////////////////////////na pewno singleton??????

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt => {
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.Authority = "https://localhost:7122/";
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

var CORSallowAny = "allowAny";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: CORSallowAny,
              policy =>
              {
                  policy.WithOrigins("https://localhost:4200", "https://127.0.0.1:5500")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
              });
});

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbContextConnString"));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DbContextConnString"));
builder.Services.AddSingleton<IUserRepositorySingleton,UserRepositorySingleton>(opt=>
{
    return new UserRepositorySingleton(optionBuilder.Options);
});

builder.Services.AddHostedService<AzureServiceBusConsumer>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            if (initialiser.Database.IsSqlServer())
                await initialiser.Database.MigrateAsync();
        }
        catch (Exception)
        {
            //_logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
}

app.UseHttpsRedirection();

app.UseCors(CORSallowAny);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MessagesHub>("/hub/Messages");

app.MapHub<PresenceHub>("/hub/Presence");

await app.RunAsync();

//app.UseAuthentication();

//app.UseSignalR(routes =>
//{
//    routes.MapHub<MainHub>("/hubs/main");
//});