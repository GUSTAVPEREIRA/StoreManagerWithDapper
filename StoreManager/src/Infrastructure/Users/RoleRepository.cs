using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Users;
using Core.Users.Interfaces;
using Dapper;
using Infrastructure.Providers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Users
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        private const string GetRoleQuery = @"SELECT id, name, is_admin FROM roles WHERE id = @id";
        private const string GetRolesQuery = @"SELECT id, name, is_admin FROM roles";

        private const string InsertRoleQuery = @"INSERT INTO roles (name, is_admin) VALUES (@name, @is_admin) RETURNING Id";

        private const string UpdateRoleQuery = @"UPDATE roles SET name = @name, is_admin = @is_admin WHERE id = @id";
        private const string DeleteRoleQuery = @"DELETE FROM roles WHERE id = @id";
        private const string CheckIfRoleExistQuery = @"SELECT id from roles WHERE id = @id"; 

        public RoleRepository(IConfiguration configuration, IDbConnectionProvider provider) : base(configuration,
            provider)
        {
        }

        public async Task<Role> GetRoleAsync(int id)
        {
            await using var connection = GetConnection();

            var role = await connection.QueryFirstOrDefaultAsync<Role>(GetRoleQuery, new
            {
                id
            });

            return role;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            await using var connection = GetConnection();

            var roles = await connection.QueryAsync<Role>(GetRolesQuery);

            return roles;
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            await using var connection = GetConnection();

            var roleId = await connection.ExecuteScalarAsync<int>(InsertRoleQuery, new
            {
                name = role.Name,
                is_admin = role.IsAdmin
            });

            role.Id = roleId;

            return role;
        }

        public async Task DeleteRoleAsync(int id)
        {
            await using var connection = GetConnection();

            await connection.ExecuteAsync(DeleteRoleQuery, new
            {
                id
            });
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {
            await using var connection = GetConnection();

            var isUpdated = await connection.ExecuteScalarAsync<int>(UpdateRoleQuery, new
            {
                name = role.Name,
                is_admin = role.IsAdmin,
                id = role.Id
            });

            return isUpdated == 1 ? role : null;
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
}