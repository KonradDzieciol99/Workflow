using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityDuende.Configuration;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
            new ApiScope("chat", "Chat Service"),
            new ApiScope("notification", "Notification Service"),
            new ApiScope("photos", "photos Service"),
            new ApiScope("project", "Project Service"),
            new ApiScope("signalR", "SignalR Service"),
            new ApiScope("tasks", "Tasks Service"),
            new ApiScope("aggregator", "Aggregator Service"),
        };
    public static IEnumerable<ApiResource> ApiResource =>
        new List<ApiResource>
        {
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            {
                Scopes = { IdentityServerConstants.LocalApi.ScopeName }
            },
            new ApiResource("chat", "Chat Service") { Scopes = { "chat" } },
            new ApiResource("notification", "Notification Service") { Scopes = { "notification" } },
            new ApiResource("photos", "photos Service") { Scopes = { "photos" } },
            new ApiResource("project", "Project Service") { Scopes = { "project" } },
            new ApiResource("signalR", "SignalR Service") { Scopes = { "signalR" } },
            new ApiResource("tasks", "Tasks Service") { Scopes = { "tasks" } },
            new ApiResource("aggregator", "Aggregator Service") { Scopes = { "aggregator" } },
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RedirectUris = { "http://localhost:4200/home", "http://localhost:1000/home" },
                //FrontChannelLogoutUri = "https://localhost:4200",
                PostLogoutRedirectUris = { "http://localhost:4200", "http://localhost:1000" },
                AllowedCorsOrigins = { "http://localhost:4200", "http://localhost:1000" },
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                CoordinateLifetimeWithUserSession = true,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    IdentityServerConstants.LocalApi.ScopeName,
                    "chat",
                    "notification",
                    "photos",
                    "project",
                    "signalR",
                    "tasks",
                    "aggregator"
                },
                RequireConsent = false,
                AllowRememberConsent = true,
                AccessTokenLifetime = 900,
            },
        };
}
