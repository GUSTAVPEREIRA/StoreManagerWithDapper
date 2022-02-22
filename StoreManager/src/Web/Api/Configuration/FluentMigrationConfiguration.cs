using System;
using Core.Configurations.Extensions;
using FluentMigrator.Runner;
using Infrastructure.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration;

public static class FluentMigrationConfiguration
{
    public static void AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSettings();

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