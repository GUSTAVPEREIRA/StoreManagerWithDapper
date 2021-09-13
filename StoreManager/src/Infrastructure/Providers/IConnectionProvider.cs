using System.Data.Common;

namespace Infrastructure.Providers
{
    public interface IDbConnectionProvider
    {
        public DbConnection GetConnection(string connection);
    }
}