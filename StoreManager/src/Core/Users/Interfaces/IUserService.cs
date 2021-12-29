using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Auth.Models;
using Core.Users.Models;

namespace Core.Users.Interfaces
{
    public interface IUserService
    {
        public Task<UserResponse> InsertUserAsync(UserRequest userRequest);
        public Task<UserResponse> UpdatedUserAsync(UserUpdatedRequest updatedRequest);
        public Task<UserResponse> GetUserAsync(int id);
        public Task<IEnumerable<UserResponse>> GetUsersAsync();
        Task<AuthUserResponse> GetUserByEmailAndPasswordAsync(AuthLoginRequest loginRequest);
    }
}