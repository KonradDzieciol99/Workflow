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

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


var app = builder.Build();



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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/api/projects/", async ([FromServices] IUnitOfWork unitOfWork,
                          [FromServices] IMapper mapper,
                          ClaimsPrincipal user,
                          [AsParameters] AppParams @params) =>
{
    var userEmail = user.FindFirstValue(ClaimTypes.Email);
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

    if (userId is null || userEmail is null)
        return Results.BadRequest("User cannot be identified.");

    var projects = await unitOfWork.ProjectMemberRepository.GetUserProjects(userId, @params);

    return Results.Ok(mapper.Map<List<ProjectDto>>(projects));

}).RequireAuthorization()
.AddEndpointFilter<ValidatorFilter<AppParams>>();

app.MapPost("api/projects/", async ([FromServices] IUnitOfWork unitOfWork,
                            [FromServices] IMapper mapper,
                             ClaimsPrincipal user,
                             [AsParameters] CreateProjectDto projectDto) =>
{
    var userEmail = user.FindFirstValue(ClaimTypes.Email);
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var userPhotUrl = user.FindFirstValue("picture");

    if (userId is null || userEmail is null)
        return Results.BadRequest("User cannot be identified.");

    var member = new ProjectMember(userId, userEmail, ProjectMemberType.Admin);

    var project = new Project(projectDto.Name, new List<ProjectMember>(){ member });

    unitOfWork.ProjectRepository.Add(project);

    if (await unitOfWork.Complete())
        return Results.Ok(mapper.Map<ProjectDto>(project));

    return Results.BadRequest("Error occurred during project creation.");

}).RequireAuthorization()
.AddEndpointFilter<ValidatorFilter<CreateProjectDto>>();

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