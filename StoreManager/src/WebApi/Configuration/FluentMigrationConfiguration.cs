using System;
using Core.Configurations;
using FluentMigrator.Runner;
using Infrastructure.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Configuration
{
    public static class FluentMigrationConfiguration
    {
        public static void AddMigrations(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSettings();
            Console.WriteLine(settings.DbConnection.PostgresConnection);

            var provider = services
                .AddFluentMigratorCore()
                .ConfigureRunner(x => x
                    .AddPostgres()
                    .WithGlobalConnectionString(settings.DbConnection.PostgresConnection)
                    .ScanIn(MigrationConfiguration.GetMigrations()).For.Migrations())
                .BuildServiceProvider(false);

            UpdateDatabase(provider);
        }

        private static void UpdateDatabase(IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            var runner = provider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}