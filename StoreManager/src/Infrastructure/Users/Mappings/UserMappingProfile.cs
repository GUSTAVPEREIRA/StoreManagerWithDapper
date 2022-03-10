using AutoMapper;
using Core.Users.Models;
using Infrastructure.Users.Models;

namespace Infrastructure.Users.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserRequest, User>().BeforeMap((userRequest, user) =>
        {
            if (userRequest.RoleId != 0)
            {
                user.Role = new Role
                {
                    Id = userRequest.RoleId
                };
            }
        });

        CreateMap<UserUpdatedRequest, User>().BeforeMap((userRequest, user) =>
        {
            if (userRequest.RoleId != 0)
            {
                user.Role = new Role
                {
                    Id = userRequest.RoleId
                };
            }
        });
        
        CreateMap<User, UserResponse>().ReverseMap();
        CreateMap<User, AuthUserResponse>();
    }
}