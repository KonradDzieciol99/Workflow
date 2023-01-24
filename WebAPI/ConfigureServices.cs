
using Application.Interfaces;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using WebAPI.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration) 
        {
            //fgh
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddTransient<ICookiesService, CookiesService>();

            //services.Configure<ApiBehaviorOptions>(options =>
            //    options.SuppressModelStateInvalidFilter = true); //WebApi Validation w controlerach 

            return services;
        }
    }
}
