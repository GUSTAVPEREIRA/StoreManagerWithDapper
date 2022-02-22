using Application.Auth;
using Application.Users;
using Core.Auth.Interfaces;
using Core.Products.Interfaces;
using Core.Users.Interfaces;
using Infrastructure.Products;
using Infrastructure.Providers;
using Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration;

public static class DependencyInjectionConfiguration
{
    public static void AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionProvider, PostgresConnectionProvider>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }
}