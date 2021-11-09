using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Users.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> CreateUser(User user);
        public Task<User> UpdateUser(User user);
        public Task<User> GetUser(int id);
        public Task<IEnumerable<User>> GetUsers();
    }
}