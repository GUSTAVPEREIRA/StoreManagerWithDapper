using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Users;
using Core.Users.Interfaces;
using Dummie.Test.Users;

namespace Repository.Test.Seeders
{
    public class UserSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly RoleSeeder _roleSeeder;

        public UserSeeder(IUserRepository userRepository, RoleSeeder roleSeeder)
        {
            _userRepository = userRepository;
            _roleSeeder = roleSeeder;
        }

        public async Task<List<User>> CreateUsers(int count)
        {
            var role = await InsertRole();
            var users = new UserDummie(role).Generate(count);

            await InsertUsers(users);

            return users;
        }

        private async Task<Role> InsertRole()
        {
            var roles = await _roleSeeder.CreateRoles(1);
            var role = roles.FirstOrDefault();
            
            return role;
        }

        private async Task InsertUsers(List<User> users)
        {
            foreach (var user in users)
            {
                await _userRepository.CreateUserAsync(user);
            }
        }
    }
}