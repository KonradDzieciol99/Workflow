//using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Projects;
using Projects.Application;
using Projects.Infrastructure;
using Projects.Infrastructure.DataAccess;
using Projects.Middleware;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebAPIServices();
builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .WriteTo.Console(theme: AnsiConsoleTheme.Code).WriteTo.Debug());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigration();
}

app.UseCors("allowAny");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.Run();

async Task ApplyMigration()
{
    using var scope = app.Services.CreateScope();
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
    return;
}