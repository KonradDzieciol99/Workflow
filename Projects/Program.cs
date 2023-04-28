using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Projects.Common;
using Projects.DataAccess;
using Projects.Entity;
using Projects.Models;
using Projects.Models.Dto;
using Projects.Repositories;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Claims;
using MessageBus.Extensions;
using MessageBus;
using MessageBus.Events;
using Projects.Endpoints.MapProjectMember;
using Projects.Common.Authorization.Requirements;
using Projects.Common.Authorization.Handlers;

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MembershipPolicy", policy =>
    policy.AddRequirements(
        new MembershipRequirement()
    ));
    options.AddPolicy("ManagementPolicy", policy =>
    policy.AddRequirements(
        new ManagementRequirement()
        ));
    options.AddPolicy("AuthorPolicy", policy =>
    policy.AddRequirements(
        new AuthorRequirement()
        ));
});

builder.Services.AddScoped<IAuthorizationHandler, ManagementRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MembershipRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AuthorRequirementHandler>();

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

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddAzureServiceBusSender(opt =>
{
    opt.ServiceBusConnectionString = builder.Configuration.GetConnectionString(name: "ServiceBusConnectionString") ?? throw new ArgumentNullException(nameof(opt.ServiceBusConnectionString));
});

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

var endpoints = app.MapGroup("/api")
                   .WithOpenApi()
                   .RequireAuthorization()
                   .AddEndpointFilter(async (invocationContext, next) =>
                   {
                        var logger = invocationContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation($"Received request for: {invocationContext.HttpContext.Request.Path}");
                        return await next(invocationContext);
                   }); 

endpoints.MapGroup("/projectMembers")
         .MapProjectMemberEnpoints();

endpoints.MapGroup("/projects")
         .MapProjectsEnpoints();
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