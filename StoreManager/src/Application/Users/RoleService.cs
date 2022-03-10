using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Users.Interfaces;
using Core.Users.Models;

namespace Application.Users;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleResponse> CreateRoleAsync(RoleRequest roleRequest)
    {
        return await _roleRepository.CreateRoleAsync(roleRequest);
    }

    public async Task<RoleResponse> UpdateRoleAsync(RoleUpdatedRequest roleUpdatedRequest)
    {
        var roleResponse = await _roleRepository.UpdateRoleAsync(roleUpdatedRequest);

        return roleResponse;
    }

    public async Task<RoleResponse> GetRoleAsync(int id)
    {
        return await _roleRepository.GetRoleAsync(id);
    }

    public async Task DeleteRoleAsync(int id)
    {
        await _roleRepository.DeleteRoleAsync(id);
    }

    public async Task<IEnumerable<RoleResponse>> GetRolesAsync()
    {
        return await _roleRepository.GetRolesAsync();
    }
}