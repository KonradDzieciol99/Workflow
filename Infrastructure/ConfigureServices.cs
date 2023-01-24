using Application.Interfaces;
using Core.Interfaces;
using Core.Interfaces.IRepositories;
using Domain.Identity.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)//, IConfiguration configuration
        {

            services.AddDbContext<ApplicationDbContext>(opt =>
                {
                    opt.UseSqlServer(configuration.GetConnectionString("DbContextConnString"));
                });
            services.AddIdentityCore<AppUser>(opt =>
                {
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireDigit = false;
                    opt.User.RequireUniqueEmail = true;
                    opt.SignIn.RequireConfirmedEmail = true;

                })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.Authority = "https://localhost:7122/";
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    //ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("ApiSettings:Secret"))),
                    //ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {

            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
            //            ValidateIssuer = false,
            //            ValidateAudience = false,
            //            ValidIssuer = configuration["JWT:Issuer"],
            //            ValidateLifetime = true,
            //            ClockSkew = TimeSpan.Zero
            //        };
            //    });

            //services.AddAuthorization(opt =>
            //{
            //    opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            //    opt.AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Admin", "Moderator"));
            //});

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ApplicationDbContextInitialiser>();

            var fromEmail = configuration["EmailConfiguration:From"];
            var sendGridKey = configuration["EmailConfiguration:SendGrid_Key"];
            services.AddHttpContextAccessor();
            services.AddFluentEmail(fromEmail)
                    .AddRazorRenderer()
                    .AddSendGridSender(sendGridKey);
            return services;
        }
    }
}
