using AutoMapper;
//using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Projects.Common;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Claims;
using MessageBus.Extensions;
using MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Net;
using Azure;
using Projects.Endpoints.Enpoints;
using Projects.Infrastructure.Repositories;
using Projects.Infrastructure.DataAccess;
using Projects.Application.Common.Mappings;
using Projects.Application.Common.Authorization.Handlers;
using Projects.Application.Common.Authorization.Requirements;
using Projects.Services;
using Projects.Infrastructure.Services;
using Projects.Application;
using Projects.Application.Common.Interfaces;
using Projects.Middleware;
using FluentValidation;

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
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("ProjectMembershipPolicy", policy =>
//    policy.AddRequirements(
//        new ProjectMembershipRequirement()
//    ));
//    options.AddPolicy("ProjectManagementPolicy", policy =>
//    policy.AddRequirements(
//        new ProjectManagementRequirement()
//        ));
//    options.AddPolicy("ProjectAuthorPolicy", policy =>
//    policy.AddRequirements(
//        new ProjectAuthorRequirement()
//        ));
//});

builder.Services.AddScoped<IAuthorizationHandler, ProjectManagementRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ProjectMembershipRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ProjectAuthorRequirementHandler>();



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

//builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddAzureServiceBusSender(opt =>
{
    opt.ServiceBusConnectionString = builder.Configuration.GetConnectionString(name: "ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
});

builder.Services.AddAzureServiceBusSubscriber(opt =>
{
    var configuration = builder.Configuration;
    opt.ServiceBusConnectionString = configuration.GetConnectionString(name: "ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
    opt.SubscriptionName = "projects";
});

builder.Services.AddMediatR(opt => opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();
builder.Services.AddScoped<IIntegrationEventService, IntegrationEventService>();

builder.Services.AddControllers();

var app = builder.Build();

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

//app.UseExceptionHandler(appError =>
//{
//    appError.Run(async context =>
//    {
//        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
//        if (exception is BadHttpRequestException myCustomException)
//        {
//            // Obs³uga konkretnego wyj¹tku
//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = 400; // Ustaw odpowiedni kod statusu
//            var response = new { message = myCustomException.Message };
//            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
//            await context.Response.WriteAsync(jsonResponse);
//        }
//        //else
//        //{
//        //    // Przekazanie kontroler do nastêpnego middleware
//        //    context.Response.StatusCode = 500; // Ustaw odpowiedni kod statusu dla innych b³êdów
//        //    await context.Response.WriteAsync("Wyst¹pi³ inny b³¹d: " + exception?.Message);
//        //}
//    });
//});
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

//var endpoints = app.MapGroup("/api")
//                   .WithOpenApi()
//                   .RequireAuthorization()
//                   .AddEndpointFilter(async (invocationContext, next) =>
//                   {
//                        var logger = invocationContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
//                        logger.LogInformation($"Received request for: {invocationContext.HttpContext.Request.Path}");
//                        return await next(invocationContext);
//                   });
//endpoints.MapGroup("/projects")
//         .MapProjectsEnpoints()
//         .MapProjectMemberEnpoints();

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