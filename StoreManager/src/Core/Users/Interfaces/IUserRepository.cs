using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Users.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> CreateUserAsync(User user);
        public Task<User> UpdateUserAsync(User user);
        public Task<User> GetUserAsync(int id);
        public Task<IEnumerable<User>> GetUsersAsync();
        public Task<User> GetUserByEmailAndPasswordAsync(string email, string password);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> ChangeUserPasswordAsync(User user);
    }
}