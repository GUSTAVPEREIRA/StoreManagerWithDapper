using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Users.Interfaces
{
    public interface IRoleRepository
    {
        public Task<Role> GetRoleAsync(int id);

        public Task<IEnumerable<Role>> GetRolesAsync();
        public Task<Role> CreateRoleAsync(Role role);
        public Task DeleteRoleAsync(int id);
        public Task<Role> UpdateRoleAsync(Role role);
    }
}