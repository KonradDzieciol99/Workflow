using AutoMapper;
using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using Tasks.Common;
using Tasks.DataAccess;
using Tasks.Entity;
using Tasks.Models;
using Tasks.Models.Dtos;
using Tasks.Repositories;
using MessageBus.Extensions;
using MessageBus.Events;
using MessageBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        ValidateAudience = false,
    };
});
builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbContextConnString"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

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


builder.Services.AddAzureServiceBusSubscriber(opt =>
{
    var configuration = builder.Configuration;
    opt.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
    opt.SubscriptionName = "tasks";
});

//builder.Services.AddAzureServiceBusSender(opt =>
//{
//    opt.ServiceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
//});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<AzureServiceBusSubscriber>();// nie potrzeba tworzyæ scope bo to singletone
var subscribeTasks = new List<Task>
{
    eventBus.Subscribe<ProjectMemberAddedEvent>(),
    eventBus.Subscribe<ProjectMemberUpdatedEvent>(),
    eventBus.Subscribe<ProjectMemberRemovedEvent>(),
};
await Task.WhenAll(subscribeTasks);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigration();
}
app.UseHttpsRedirection();
app.UseCors(CORSallowAny);
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api",async ([FromServices] IUnitOfWork unitOfWork,
                          [FromServices] IMapper mapper,
                          ClaimsPrincipal user,
                          HttpContext context,
                          [AsParameters] CreateAppTaskDto createAppTask) =>
{

    var userEmail = user.FindFirstValue(ClaimTypes.Email);
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var userPhotUrl = user.FindFirstValue("picture");

    if (userId is null || userEmail is null)
        return Results.BadRequest("User cannot be identified.");

    var projectsServiceResult = await unitOfWork.ProjectMemberRepository.CheckIfUserIsAMemberOfProject(createAppTask.ProjectId, userId);

    if (projectsServiceResult == false) { 
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("You are not a member of this project");
    }

    var appTask = mapper.Map<AppTask>(createAppTask);

    unitOfWork.AppTaskRepository.Add(appTask);

    if (await unitOfWork.Complete())
        return Results.Ok(appTask);
        
    return Results.BadRequest("Task could not be added.");
})
//ValidationFilter
.WithOpenApi()
.RequireAuthorization();


app.Run();

async Task ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            if (initialiser.Database.IsSqlServer())
                await initialiser.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    await Task.CompletedTask;
    return;
}
