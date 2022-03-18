using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Users;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using Infrastructure.Users.Models;

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

        public async Task<List<UserResponse>> CreateUsers(int count)
        {
            var role = await InsertRole();
            var userRequests = new UserRequestDummie(role.Id).Generate(count);

            return await InsertUsers(userRequests);
        }

        private async Task<RoleResponse> InsertRole()
        {
            var roles = await _roleSeeder.CreateRoles(1);
            var role = roles.FirstOrDefault();
            
            return role;
        }

        private async Task<List<UserResponse>> InsertUsers(List<UserRequest> users)
        {
            List<UserResponse> userResponses = new(users.Count);
            
            foreach (var user in users)
            {
                userResponses.Add(await _userRepository.CreateUserAsync(user));
            }

            return userResponses;
        }
    }
}