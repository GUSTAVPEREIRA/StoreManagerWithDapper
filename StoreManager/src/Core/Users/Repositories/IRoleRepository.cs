using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Users.Repositories
{
    public interface IRoleRepository
    {
        public Task<Role> GetRoleAsync(int id);

        public Task<List<Role>> GetRolesAsync();
        public Task<Role> CreateRoleAsync();
        public Task<Role> DeleteRoleAsync();
        public Task<Role> UpdateRoleAsync();
    }
}