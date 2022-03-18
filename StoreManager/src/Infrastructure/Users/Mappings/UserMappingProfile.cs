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

        CreateMap<User, UserResponse>().BeforeMap((user, userResponse) =>
        {
            if (user.Role != null)
            {
                userResponse.Role = new RoleResponse
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name,
                    IsAdmin = user.Role.IsAdmin
                };
            }
        });
        CreateMap<UserResponse, User>();
        CreateMap<User, AuthUserResponse>();
    }
}