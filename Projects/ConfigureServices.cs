
using Microsoft.AspNetCore.Mvc;
using Projects.Application.Common.Interfaces;
using Projects.Services;

namespace Projects;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
