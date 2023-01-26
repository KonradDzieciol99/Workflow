using Messenger.API;
using Messenger.API.Persistence;
using Messenger.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    options.Configuration.ChannelPrefix = "MessengerSignalR";
});

var RedisOptions = new ConfigurationOptions()
{
    EndPoints = { { builder.Configuration.GetConnectionString("Redis") } },
    IncludeDetailInExceptions = true,
};

builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(RedisOptions)
);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.Authority = "https://localhost:7122/";
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CORSallowAny);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MessagesHub>("/hub/Messages");

app.Run();

