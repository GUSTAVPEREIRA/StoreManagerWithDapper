using Infrastructure.Users.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations;

public static class AutoMapperConfiguration
{
    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(RoleMappingProfile));
        services.AddAutoMapper(typeof(UserMappingProfile));
    }
}