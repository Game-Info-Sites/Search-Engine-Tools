using Microsoft.Extensions.Logging;
using SearchEngineTools.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace SearchEngineTools.Migrations
{

    public class CreateSearchEngineSubmissionQueueTable(IMigrationContext context) : MigrationBase(context)
    {
        protected override void Migrate()
        {
            Logger.LogInformation("Running migration {MigrationStep}", "CreateSearchEngineSubmissionQueueTable");

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
    }
}
