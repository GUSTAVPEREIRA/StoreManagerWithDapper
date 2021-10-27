using System;
using FluentMigrator.Runner;
using Infrastructure.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Repository.Test.Configuration
{
    public static class DatabaseConfiguration
    {
        public static void CreateMigrations(string database)
        {
            var connectionString = GetConnectionString(database);
            var service = CreateServiceProvider(connectionString);
            
            using var scope = service.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }
        
        public static void RemoveMigrations(string database)
        {
            var connectionString = GetConnectionString(database);
            var service = CreateServiceProvider(connectionString);
            
            using var scope = service.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateDown(-1);
        }

        public static string GetConnectionString(string database)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new NullReferenceException("Nome do banco não pode ser nullo!");
            }

            var appContextPath = AppContext.BaseDirectory;
            var connectionString = $"Data Source={appContextPath}/{database}.db; Version=3;";

            return connectionString;
        }

        private static IServiceProvider CreateServiceProvider(string connectionString)
        {
            var service = new ServiceCollection()
                .AddLogging(x => x.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(builder =>
                    builder
                        .AddSQLite()
                        .WithGlobalConnectionString(connectionString)
                        .WithMigrationsIn(MigrationConfiguration.GetMigrations()))
                .BuildServiceProvider();

            return service;
        }
    }
}