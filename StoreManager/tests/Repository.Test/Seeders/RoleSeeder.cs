using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Users;
using Core.Users.Repositories;
using Dummies.Test.Users;

namespace Repository.Test.Seeders
{
    public class RoleSeeder
    {
        private readonly IRoleRepository _roleRepository;

        public RoleSeeder(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> CreateRoles(int count)
        {
            var roles = new RoleDummie().Generate(count);

            return await InsertRoles(roles);
        }

        private async Task<List<Role>> InsertRoles(List<Role> roles)
        {
            foreach (var role in roles)
            {
                await _roleRepository.CreateRoleAsync(role);
            }

            return roles;
        }
    }
}