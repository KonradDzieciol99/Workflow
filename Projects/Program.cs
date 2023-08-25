//using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Projects.Application;
using Projects.Infrastructure.DataAccess;
using Projects.Middleware;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();

// Use Serilog
//builder.Host.UseSerilog((hostContext, services, configuration) => {
//    configuration
//        //.WriteTo.File("serilog-file.txt")
//        .WriteTo.Console();
//});

//var Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .WriteTo.Console()
//    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(Logger);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//builder.Host.UseSerilog((ctx, lc) => lc
//    .WriteTo.Console());

//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));




builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebAPIServices();

//builder.Host.UseSerilog((context,config) => config.MinimumLevel.Debug().WriteTo.Console(theme: AnsiConsoleTheme.Code));

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .WriteTo.Console(theme: AnsiConsoleTheme.Code).WriteTo.Debug());
//builder.Services.AddLogging();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigration();
}

//app.UseHttpsRedirection();

app.UseCors("allowAny");

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

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