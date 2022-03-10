using AutoMapper;
using Core.Users.Models;
using Infrastructure.Users.Models;

namespace Infrastructure.Users.Mappings;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<RoleRequest, Role>().ReverseMap();
        CreateMap<RoleUpdatedRequest, Role>().ReverseMap();
        CreateMap<Role, RoleResponse>();
    }
}