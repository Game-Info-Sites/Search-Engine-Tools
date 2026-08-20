using Microsoft.Extensions.Logging;
using SearchEngineTools.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace SearchEngineTools.Migrations
{
    public class EnsureSearchEngineToolsTables(IMigrationContext context) : MigrationBase(context)
    {
        protected override void Migrate()
        {
            Logger.LogInformation("Running migration {MigrationStep}", "EnsureSearchEngineToolsTables");

            EnsureSearchEngineSubmissionQueueTable();
            EnsureIndexNowKeysTable();
        }

        private void EnsureSearchEngineSubmissionQueueTable()
        {
            if (TableExists("SearchEngineSubmissionQueue"))
            {
                return;
            }

            Create.Table("SearchEngineSubmissionQueue")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Url").AsString(SearchEngineSubmissionItem.MaxUrlLength).NotNullable().Unique()
                .WithColumn("Status").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("LastModifiedUtc").AsDateTime().NotNullable()
                .WithColumn("LastSubmittedUtc").AsDateTime().Nullable()
                .WithColumn("LastAttemptUtc").AsDateTime().Nullable()
                .WithColumn("RetryCount").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("LastError").AsString(SearchEngineSubmissionItem.MaxErrorLength).Nullable()
                .Do();
        }

        private void EnsureIndexNowKeysTable()
        {
            if (TableExists("IndexNowKeys"))
            {
                return;
            }

            Create.Table("IndexNowKeys")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Domain").AsString(IndexNowKey.MaxDomainLength).NotNullable().Unique()
                .WithColumn("Key").AsString(IndexNowKey.MaxKeyLength).NotNullable()
                .WithColumn("CreatedUtc").AsDateTime().NotNullable()
                .Do();
        }
    }
}
