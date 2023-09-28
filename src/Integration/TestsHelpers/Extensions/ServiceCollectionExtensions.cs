using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsHelpers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Remove<TService>(this IServiceCollection services)
    {
        var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));

        if (serviceDescriptor != null)
            services.Remove(serviceDescriptor);

        return services;
    }
}
