using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityServer.Events;
using IdentityServer.Persistence;
using IdentityServer.Services;
using Mango.MessageBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//var migrationsAssembly = typeof(Config).Assembly.GetName().Name;

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
    options.UseSqlServer(configuration.GetConnectionString("DbContextConnString"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
    //opt.Password.RequireNonAlphanumeric = false;
    //opt.Password.RequireUppercase = false;
    //opt.Password.RequireDigit = false;
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;

    //options.UserInteraction.LogoutUrl = "/home";

}).AddInMemoryIdentityResources(Config.IdentityResources)
.AddInMemoryApiResources(Config.ApiResources)
.AddInMemoryApiScopes(Config.ApiScopes)
.AddInMemoryClients(Config.Clients)
.AddAspNetIdentity<IdentityUser>()
.AddDeveloperSigningCredential();

//.AddProfileService<ProfileService>(); ////narazie nie robi nic ponad ten zwyk�y
//builder.Services.AddScoped<IProfileService, ProfileService>(); ////narazie nie robi nic ponad ////narazie nie robi nic ponad ten zwyk�y

//.AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
//opt => opt.MigrationsAssembly(migrationsAssembly)))
//.AddOperationalStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
//opt => opt.MigrationsAssembly(migrationsAssembly)))

builder.Services.AddAuthentication().AddMicrosoftAccount(opt =>
{
    opt.ClientId = configuration["Authentication:Microsoft:ClientId"];
    opt.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
    opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
});

builder.Services.AddScoped<IEventSink, IdentityEvents>();

builder.Services.AddScoped<ApplicationDbContextInitialiser>();

builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseIdentityServer();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

await app.RunAsync();