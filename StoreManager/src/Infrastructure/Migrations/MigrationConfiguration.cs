using System.Reflection;

namespace Infrastructure.Migrations;

public static class MigrationConfiguration
{
    public static Assembly[] GetMigrations()
    {
        var assemblies = new[]
        {
            typeof(CreateRoleTable).Assembly,
            typeof(CreateUserTable).Assembly,
            typeof(CreateProductsTable).Assembly
        };

        return assemblies;
    }
}