using System.Data.Common;
using Core.Configurations.Extensions;
using Infrastructure.Providers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public abstract class BaseRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration, IDbConnectionProvider provider)
        {
            _dbConnectionProvider = provider;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionString = configuration.GetConnectionsString();
        }

        protected DbConnection GetConnection()
        {
            return _dbConnectionProvider.GetDbConnection(_connectionString);
        }
    }
}