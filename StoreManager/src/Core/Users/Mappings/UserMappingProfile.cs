using AutoMapper;
using Core.Users.Models;

namespace Core.Users.Mappings
{
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
            CreateMap<UserUpdatedRequest, User>();
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<User, AuthUserResponse>();
        }
    }
}