using System.Data.Common;
using Npgsql;

namespace Infrastructure.Providers
{
    public class PosgresConnectionProvider : IDbConnectionProvider
    {
        public DbConnection GetConnection(string connection)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            return new NpgsqlConnection(connection);
        }
    }
}