using Application.Users;
using Core.Users.Interfaces;
using Infrastructure.Providers;
using Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IDbConnectionProvider, PostgresConnectionProvider>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();
        }
    }
}