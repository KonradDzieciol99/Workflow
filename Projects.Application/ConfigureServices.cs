using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Projects.Application.Common.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Projects.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Projects.Application.Common.Authorization.Handlers;
using Projects.Application.Common.Mappings;

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
