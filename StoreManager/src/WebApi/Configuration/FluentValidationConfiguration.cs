using System.Globalization;
using System.Text.Json.Serialization;
using Core.Errors.Middlewares;
using Core.Users.Models;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebApi.Configuration
{
    public static class FluentValidationConfiguration
    {
        public static void AddFluentValidationConfiguration(this IServiceCollection service)
        {
            service.AddControllers(options =>
                {
                    options.Filters.Add(new GlobalExceptionMiddleware()); 
                    
                })
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddJsonOptions(x => { x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
                .AddFluentValidation(x =>
                {
                    x.RegisterValidatorsFromAssemblyContaining<RoleRequestValidation>();
                    x.RegisterValidatorsFromAssemblyContaining<RoleUpdatedRequestValidation>();
                    x.ValidatorOptions.LanguageManager.Culture = new CultureInfo("pt-BR");
                });
        }
    }
}