using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SearchEngineTools.Data;
using Umbraco.Cms.Infrastructure.Migrations;

namespace SearchEngineTools.Migrations
{
    public class RunEfCoreMigrations(
        IMigrationContext context,
        IServiceScopeFactory scopeFactory) : MigrationBase(context)
    {
        private const string InitialMigrationId = "20260630000100_InitialSearchEngineSubmissionQueue";
        private const string AddIndexNowKeysMigrationId = "20260630152139_AddIndexNowKeys";
        private const string EfProductVersion = "10.0.0";

        protected override void Migrate()
        {
            Logger.LogInformation("Running migration {MigrationStep}", "RunEfCoreMigrations");

            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SearchEngineToolsDbContext>();

            EnsureBaselineForExistingInstall(dbContext);
            dbContext.Database.Migrate();
        }

        private static void EnsureBaselineForExistingInstall(SearchEngineToolsDbContext dbContext)
        {
            if (!dbContext.Database.IsSqlite())
            {
                return;
            }

            using var connection = dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var submissionQueueTableExists = TableExists(connection, "SeoSubmissionQueue");
            var indexNowKeysTableExists = TableExists(connection, "IndexNowKeys");
            if (!submissionQueueTableExists && !indexNowKeysTableExists)
            {
                return;
            }

            dbContext.Database.ExecuteSqlRaw(
                """
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                "ProductVersion" TEXT NOT NULL
            );
            """);

            if (submissionQueueTableExists)
            {
                InsertMigrationHistory(dbContext, InitialMigrationId);
            }

            if (indexNowKeysTableExists)
            {
                InsertMigrationHistory(dbContext, AddIndexNowKeysMigrationId);
            }
        }

        private static bool TableExists(DbConnection connection, string tableName)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type = 'table' AND name = $tableName;";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "$tableName";
            parameter.Value = tableName;
            command.Parameters.Add(parameter);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        private static void InsertMigrationHistory(SearchEngineToolsDbContext dbContext, string migrationId)
        {
            dbContext.Database.ExecuteSqlRaw(
                """
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            SELECT {0}, {1}
            WHERE NOT EXISTS (
                SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = {0}
            );
            """,
                migrationId,
                EfProductVersion);
        }
    }

}
