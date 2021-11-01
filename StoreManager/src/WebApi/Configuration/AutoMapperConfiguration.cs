using Core.Users.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(RoleMappingProfile));
        }
    }
}