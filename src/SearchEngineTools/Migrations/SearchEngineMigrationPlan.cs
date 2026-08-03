using Umbraco.Cms.Infrastructure.Migrations;

namespace SearchEngineTools.Migrations
{
    public class SearchEngineMigrationPlan : MigrationPlan
    {
        public SearchEngineMigrationPlan() : base("SearchEngineTools")
        {
            From(string.Empty).To<RunEfCoreMigrations>("run-ef-core-migrations");
            From("create-search-engine-submission-table").To<RunEfCoreMigrations>("run-ef-core-migrations");
        }
    }
}
