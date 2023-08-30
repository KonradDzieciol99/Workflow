using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Projects.Application.Common.Authorization.Handlers;
using Projects.Application.Common.Behaviours;
using Projects.Application.Common.Mappings;
using System.Reflection;

namespace Projects.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        services.AddScoped<IAuthorizationHandler, ProjectManagementRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectMembershipRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectAuthorRequirementHandler>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        });

        return services;
    }
}
