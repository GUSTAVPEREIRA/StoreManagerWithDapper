using Core.Users.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(RoleMappingProfile));
            services.AddAutoMapper(typeof(UserMappingProfile));
        }
    }
}