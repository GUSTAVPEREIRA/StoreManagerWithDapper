using AutoMapper;
using Core.Users.Models;

namespace Core.Users.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserRequest, User>();
            CreateMap<UserUpdatedRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}