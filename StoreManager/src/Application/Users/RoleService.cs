using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Users;
using Core.Users.Interfaces;
using Core.Users.Models;

namespace Application.Users;

public class RoleService : IRoleService
{
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;

    public RoleService(IMapper mapper, IRoleRepository roleRepository)
    {
        _mapper = mapper;
        _roleRepository = roleRepository;
    }

    public async Task<RoleResponse> CreateRoleAsync(RoleRequest roleRequest)
    {
        var role = _mapper.Map<Role>(roleRequest);
        var roleResponse = _mapper.Map<RoleResponse>(await _roleRepository.CreateRoleAsync(role));

        return roleResponse;
    }

    public async Task<RoleResponse> UpdateRoleAsync(RoleUpdatedRequest roleUpdatedRequest)
    {
        var role = _mapper.Map<Role>(roleUpdatedRequest);
        var roleResponse = _mapper.Map<RoleResponse>(await _roleRepository.UpdateRoleAsync(role));

        return roleResponse;
    }

    public async Task<RoleResponse> GetRoleAsync(int id)
    {
        var role = await _roleRepository.GetRoleAsync(id);
        return _mapper.Map<RoleResponse>(role);
    }

    public async Task DeleteRoleAsync(int id)
    {
        await _roleRepository.DeleteRoleAsync(id);
    }

    public async Task<IEnumerable<RoleResponse>> GetRolesAsync()
    {
        var roles = await _roleRepository.GetRolesAsync();
        return _mapper.Map<IEnumerable<RoleResponse>>(roles);
    }
}