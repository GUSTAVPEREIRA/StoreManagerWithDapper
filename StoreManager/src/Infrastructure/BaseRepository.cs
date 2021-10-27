using System.Data.Common;
using Core.Configurations.Extensions;
using Infrastructure.Providers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public abstract class BaseRepository
    {
        protected readonly IDbConnectionProvider DbConnectionProvider;
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration, IDbConnectionProvider provider)
        {
            DbConnectionProvider = provider;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionString = configuration.GetConnectionsString();
        }

        public DbConnection GetConnection()
        {
            return DbConnectionProvider.GetDbConnection(_connectionString);
        }
    }
}