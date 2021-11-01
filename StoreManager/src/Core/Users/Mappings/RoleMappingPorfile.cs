using AutoMapper;
using Core.Users.Models;

namespace Core.Users.Mappings
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<RoleRequest, Role>();
            CreateMap<RoleUpdatedRequest, Role>();
            CreateMap<Role, RoleResponse>();
        }
    }
}