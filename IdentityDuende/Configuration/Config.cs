using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

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
                RequirePkce= true,
                RedirectUris = { "https://localhost:4200/home" },
                FrontChannelLogoutUri = "https://localhost:4200",
                PostLogoutRedirectUris = { "https://localhost:4200" },
                AllowedCorsOrigins = { "https://localhost:4200" },
                AllowOfflineAccess = true,
                ////UpdateAccessTokenClaimsOnRefresh = true, TO AAKURAt ciekawe
                //AlwaysIncludeUserClaimsInIdToken = true, dotyczy Id Tokena
                //AlwaysSendClientClaims= true, dotyczy Id Tokena
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
                    "tasks"
                },
                RequireConsent = false,
                AllowRememberConsent = true,
                AccessTokenLifetime = 6000,
            },
        };
}
