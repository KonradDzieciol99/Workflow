using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityServer.Events;
using IdentityServer.Persistence;
using IdentityServer.Services;
using Mango.MessageBus;
using MessageBus.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using Polly;
using Polly.Extensions.Http;
using IdentityServer.Repositories;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//var migrationsAssembly = typeof(Config).Assembly.GetName().Name;
builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
    options.UseSqlServer(configuration.GetConnectionString("DbContextConnString"));
});

builder.Services.AddIdentity</*IdentityUser*/ AppUser, IdentityRole>(opt =>
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
.AddAspNetIdentity<AppUser/*IdentityUser*/>()
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
//.AddJwtBearer(opt =>
//{
//    opt.RequireHttpsMetadata = false;
//    opt.SaveToken = true;
//    opt.Authority = "https://localhost:7122/";
//    //opt.TokenValidationParameters = new TokenValidationParameters
//    //{
//    //    ValidateAudience = false,
//    //};
//    opt.Audience = IdentityServerConstants.LocalApi.ScopeName;
//});
//builder.Services.AddAuthentication(opt =>
//{
//    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(opt =>
//{
//    opt.RequireHttpsMetadata = false;
//    opt.SaveToken = true;
//    opt.Authority = "https://localhost:7122/";
//    opt.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateAudience = false,
//    };
//});

builder.Services.AddLocalApiAuthentication();
//https://lurumad.github.io/aditional-api-endpoints-to-identityserver4
//https://docs.duendesoftware.com/identityserver/v6/apis/add_apis/

builder.Services.AddScoped<IEventSink, IdentityEvents>();

builder.Services.AddScoped<ApplicationDbContextInitialiser>();

//builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();
builder.Services.AddAzureServiceBusSender(opt =>
{
    opt.ServiceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBusConnectionString");
});

builder.Services.AddHttpClient<IChatServiceHttpRequest, ChatServiceHttpRequest>(c => 
{
    c.BaseAddress = new Uri("https://localhost:7282");
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());
    //.AddHttpMessageHandler<LoggingDelegatingHandler>()

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(CORSallowAny);

app.UseIdentityServer();

app.UseStaticFiles();
app.UseRouting();
//app.UseAuthorization();

app.UseAuthentication();

app.UseAuthorization();
//

app.MapRazorPages().RequireAuthorization();

app.MapControllers().RequireAuthorization();
    //.RequireAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=IdentityUser}/{action=test22}/{id?}");

//app.MapControllerRoute(
//    name: "IdentityUser",
//    pattern: "IdentityUser/search/{emailOfSearchedUser}",
//    defaults: new { controller = "IdentityUser", action = "search" });

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



static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    // In this case will wait for
    //  2 ^ 1 = 2 seconds then
    //  2 ^ 2 = 4 seconds then
    //  2 ^ 3 = 8 seconds then
    //  2 ^ 4 = 16 seconds then
    //  2 ^ 5 = 32 seconds

    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, retryCount, context) =>
            {
                //Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                Console.WriteLine($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)
        );
}