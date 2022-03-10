using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Users.Models;

namespace Core.Users.Interfaces;

public interface IRoleRepository
{
    public Task<RoleResponse> GetRoleAsync(int id);

    public Task<IEnumerable<RoleResponse>> GetRolesAsync();
    public Task<RoleResponse> CreateRoleAsync(RoleRequest roleRequest);
    public Task DeleteRoleAsync(int id);
    public Task<RoleResponse> UpdateRoleAsync(RoleUpdatedRequest roleRequest);
    public Task<bool> CheckIfRoleExist(int id);
}