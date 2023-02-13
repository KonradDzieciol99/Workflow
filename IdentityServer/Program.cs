using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityServer.Events;
using IdentityServer.Persistence;
using IdentityServer.Services;
using Mango.MessageBus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using static System.Formats.Asn1.AsnWriter;

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
    //options.
    //options.GetClaimsFromUserInfoEndpoint = true;
    //options.UserInteraction.LogoutUrl = "/home";

}).AddInMemoryIdentityResources(Config.IdentityResources)
.AddInMemoryApiResources(Config.ApiResources)
.AddInMemoryApiScopes(Config.ApiScopes)
.AddInMemoryClients(Config.Clients)
.AddAspNetIdentity<IdentityUser>()
.AddDeveloperSigningCredential();

//.AddProfileService<ProfileService>(); ////narazie nie robi nic ponad ten zwyk³y
//builder.Services.AddScoped<IProfileService, ProfileService>(); ////narazie nie robi nic ponad ////narazie nie robi nic ponad ten zwyk³y

//.AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
//opt => opt.MigrationsAssembly(migrationsAssembly)))
//.AddOperationalStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
//opt => opt.MigrationsAssembly(migrationsAssembly)))

builder.Services.AddAuthentication()
.AddOpenIdConnect("AzureOpenId", "Azure Active Directory OpenId", options =>
{
    //options.Authority = "https://login.microsoftonline.com/" + configuration["AzureAd:TenantId"] + "/v2.0/";
    options.Authority = "https://login.microsoftonline.com/" + "5d3aafdf-d077-4419-bd3c-622d8000bc09" + "/v2.0/";
    options.ClientId = configuration["Authentication:Microsoft:ClientId"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.CallbackPath = "/signin-microsoft";
    //            options.UsePkce = _identityServerConfiguration.UsePkce;

    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
    options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

    //options.CallbackPath = "https://localhost:7122/signin-microsoft";
    //options.CallbackPath = configuration["AzureAd:CallbackPath"];
    options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
    options.RequireHttpsMetadata = false;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("openid");
    options.Scope.Add("User.Read");

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub"); 
})
//.AddMicrosoftAccount(opt =>
//{
//    opt.ClientId = configuration["Authentication:Microsoft:ClientId"];
//    opt.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
//    opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
//    opt.get
//    //opt.Events=
//});

.AddOpenIdConnect("Google", "Sign-in with Google", options =>
{
    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
    options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

    options.Authority = "https://accounts.google.com/";
    options.ClientId = "944959518171-o303j7ij1434c3j1mrcl983qedr167b9.apps.googleusercontent.com";//todo
    options.ClientSecret = "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";//todo
    options.CallbackPath = "/signin-google";//todo
    options.GetClaimsFromUserInfoEndpoint = true;//We need this option
    options.RequireHttpsMetadata = false;
    options.SaveTokens = true;
    
    //options.Scope.Add("email");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("openid");
    options.ResponseType = OpenIdConnectResponseType.Code;

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