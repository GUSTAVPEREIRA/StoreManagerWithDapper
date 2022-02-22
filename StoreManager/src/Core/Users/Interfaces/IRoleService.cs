using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Users.Models;

namespace Core.Users.Interfaces;

public interface IRoleService
{
    public Task<RoleResponse> CreateRoleAsync(RoleRequest roleRequest);
    public Task<RoleResponse> UpdateRoleAsync(RoleUpdatedRequest roleUpdatedRequest);
    public Task<RoleResponse> GetRoleAsync(int id);
    public Task DeleteRoleAsync(int id);
    public Task<IEnumerable<RoleResponse>> GetRolesAsync();
}