using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;

namespace Repository.Test.Seeders
{
    public class RoleSeeder
    {
        private readonly IRoleRepository _roleRepository;

        public RoleSeeder(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleResponse>> CreateRoles(int count)
        {
            var roleRequests = new RoleRequestDummie().Generate(count);

            return await InsertRoles(roleRequests);
        }

        private async Task<List<RoleResponse>> InsertRoles(List<RoleRequest> roleRequests)
        {
            List<RoleResponse> roleResponses = new(roleRequests.Count);
            
            foreach (var role in roleRequests)
            {
                roleResponses.Add(await _roleRepository.CreateRoleAsync(role));
            }

            return roleResponses;
        }
    }
}