﻿using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;
using System.Security.Claims;
using System.Text.Json;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
            //new IdentityResource("custom.profile",
            //userClaims: new[] { JwtClaimTypes.Name, JwtClaimTypes.Email, "location"})
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("weatherapi.read"),
            new ApiScope("weatherapi.write"),
            new ApiScope(name: "email_access_token",displayName: "User Email.", userClaims: new[] { JwtClaimTypes.Email }),
        };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("weatherapi")
        {
            Scopes = new List<string> {"weatherapi.read", "weatherapi.write"},
            ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
            UserClaims = new List<string> {"role"}
        },
        //new ApiResource("customer", "Customer API")
        //new ApiResource("email")
        //{
        //Scopes = { "customer.read", "customer.contact", "manage", "enumerate" },  
        //UserClaims ={

        //"department_id",
        //"sales_region"
        //}
        //}// additional claims to put into access token
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
                //    AlwaysIncludeUserClaimsInIdToken = true,
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
                //AllowedScopes = { "openid", "profile", "weatherapi.read" },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    //"api1",
                    IdentityServerConstants.StandardScopes.Email,
                    "email_access_token"
                    //"weatherapi.read"
                    //"custom.profile",
                },
                
                RequireConsent = true,
                AllowRememberConsent = true,/// ?????
                AccessTokenLifetime = 600,
                
            },
        };
    public static List<TestUser> Users ////one są ważne jeśli nie mamy dodanego Identyty do projektu jeśli mamy dodane to usermenager
    {
        get
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };

            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "alice",
                    
                    Claims =
                    {

                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        //new Claim(JwtClaimTypes., "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "bob",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };
        }
    }
}
