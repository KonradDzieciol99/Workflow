using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace Identity;

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
            new ApiScope("weatherapi.read"),
            new ApiScope("weatherapi.write"),
            new ApiScope(name: "email_access_token",displayName: "User Email.", userClaims: new[] { JwtClaimTypes.Email }),
            new ApiScope(name: "picture_access_token",displayName: "User Picture.", userClaims: new[] { JwtClaimTypes.Picture }),
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
        };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("weatherapi")
        {
            Scopes = new List<string> {"weatherapi.read", "weatherapi.write"},
            ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
            UserClaims = new List<string> {"role"}
        },
    };
    
    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedScopes = {"weatherapi.read", "weatherapi.write"},
            },

            // interactive client using code flow + pkce
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
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "email_access_token",
                    IdentityServerConstants.LocalApi.ScopeName,
                    "picture_access_token",
                },
                
                RequireConsent = false,//// ????
                AllowRememberConsent = true,
                AccessTokenLifetime = 6000,
                
            },
        };
}
