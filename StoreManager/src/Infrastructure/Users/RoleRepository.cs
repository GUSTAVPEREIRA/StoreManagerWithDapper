using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dapper;
using Infrastructure.Providers;
using Infrastructure.Users.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Users;

public class RoleRepository : BaseRepository, IRoleRepository
{
    private readonly IMapper _mapper;

    private const string GetRoleQuery = @"SELECT id, name, is_admin FROM roles WHERE id = @id";
    private const string GetRolesQuery = @"SELECT id, name, is_admin FROM roles";

    private const string InsertRoleQuery = @"INSERT INTO roles (name, is_admin) VALUES (@name, @is_admin) RETURNING Id";

    private const string UpdateRoleQuery =
        @"UPDATE roles SET name = @name, is_admin = @is_admin WHERE id = @id RETURNING Id";

    private const string DeleteRoleQuery = @"DELETE FROM roles WHERE id = @id";
    private const string CheckIfRoleExistQuery = @"SELECT id from roles WHERE id = @id";

    public RoleRepository(IConfiguration configuration, IDbConnectionProvider provider, IMapper mapper) : base(
        configuration,
        provider)
    {
        _mapper = mapper;
    }

    public async Task<RoleResponse> GetRoleAsync(int id)
    {
        await using var connection = GetConnection();

        var role = await connection.QueryFirstOrDefaultAsync<Role>(GetRoleQuery, new
        {
            id
        });

        return _mapper.Map<Role, RoleResponse>(role);
    }

    public async Task<IEnumerable<RoleResponse>> GetRolesAsync()
    {
        await using var connection = GetConnection();
        var roles = await connection.QueryAsync<Role>(GetRolesQuery);

        return _mapper.Map<IEnumerable<Role>, IEnumerable<RoleResponse>>(roles);
    }

    public async Task<RoleResponse> CreateRoleAsync(RoleRequest roleRequest)
    {
        await using var connection = GetConnection();
        var role = _mapper.Map<RoleRequest, Role>(roleRequest);

        var roleId = await connection.ExecuteScalarAsync<int>(InsertRoleQuery, new
        {
            name = role.Name,
            is_admin = role.IsAdmin
        });

        role.Id = roleId;

        return _mapper.Map<Role, RoleResponse>(role);
    }

    public async Task DeleteRoleAsync(int id)
    {
        await using var connection = GetConnection();

        await connection.ExecuteAsync(DeleteRoleQuery, new
        {
            id
        });
    }

    public async Task<RoleResponse> UpdateRoleAsync(RoleUpdatedRequest roleUpdatedRequest)
    {
        await using var connection = GetConnection();
        var role = _mapper.Map<RoleUpdatedRequest, Role>(roleUpdatedRequest);

        var isUpdated = await connection.ExecuteScalarAsync<int>(UpdateRoleQuery, new
        {
            name = role.Name,
            is_admin = role.IsAdmin,
            id = role.Id
        });

        return isUpdated > 0 ? _mapper.Map<Role, RoleResponse>(role) : null;
    }

    public async Task<bool> CheckIfRoleExist(int id)
    {
        await using var connection = GetConnection();

        var roleExist = await connection.QueryFirstOrDefaultAsync<int>(CheckIfRoleExistQuery, new
        {
            id
        });

        return roleExist >= 1;
    }
}