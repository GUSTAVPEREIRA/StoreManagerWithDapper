using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Users.Models;

namespace Core.Users.Interfaces;

public interface IUserRepository
{
    public Task<UserResponse> CreateUserAsync(UserRequest userRequest);
    public Task<UserResponse> UpdateUserAsync(UserUpdatedRequest userRequest);
    public Task<UserResponse> GetUserAsync(int id);
    public Task<IEnumerable<UserResponse>> GetUsersAsync();
    public Task<AuthUserResponse> GetUserByEmailAndPasswordAsync(string email, string password);
    public Task<UserResponse> GetUserByEmailAsync(string email);
    public Task<UserResponse> ChangeUserPasswordAsync(UserUpdatedRequest userUpdatedRequest);
}