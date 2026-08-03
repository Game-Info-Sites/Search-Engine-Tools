using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchEngineTools.BackgroundServices;
using SearchEngineTools.Configuration;
using SearchEngineTools.Data;
using SearchEngineTools.Handlers;
using SearchEngineTools.Middleware;
using SearchEngineTools.Migrations;
using SearchEngineTools.Repositories;
using SearchEngineTools.Services;
using SearchEngineTools.Services.Providers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace SearchEngineTools.Composers
{
    public class SearchEngineMigrationComposer :IComposer
    {

        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, SearchEngineMigrationHandler>();
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, IndexNowDomainNotificationHandler>();

            builder.AddNotificationAsyncHandler<ContentPublishedNotification, ContentPublishedHandler>();
            builder.AddNotificationAsyncHandler<DomainSavedNotification, IndexNowDomainNotificationHandler>();
            builder.AddNotificationAsyncHandler<DomainDeletedNotification, IndexNowDomainNotificationHandler>();

            builder.Services.Configure<IndexNowOptions>(
                builder.Config.GetSection(IndexNowOptions.SectionName));

            builder.Services.Configure<ThrottlingOptions>(
                builder.Config.GetSection(ThrottlingOptions.SectionName));

            builder.Services.AddDbContext<SearchEngineToolsDbContext>(options =>
            {
                var connectionString =
                    builder.Config.GetConnectionString("umbracoDbDSN");

                connectionString = connectionString.Replace(
                    "|DataDirectory|",
                    Directory.GetCurrentDirectory());

                options.UseSqlite(connectionString);
            });

            builder.Services.AddScoped<ISearchEngineSubmissionQueueRepository, SearchEngineSubmissionQueueRepository>();
            builder.Services.AddScoped<IIndexNowKeyService, IndexNowKeyService>();
            builder.Services.AddTransient<IStartupFilter, IndexNowKeyVerificationStartupFilter>();

            builder.Services.AddHttpClient<IndexNowSubmissionService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            });
            builder.Services.AddScoped<IIndexNowSubmissionService>(service => service.GetRequiredService<IndexNowSubmissionService>());
            builder.Services.AddScoped<ISearchEngineSubmissionProvider>(service => service.GetRequiredService<IndexNowSubmissionService>());

            builder.Services.AddHostedService<SearchEngineSubmissionQueueWorker>();

        }
    }

    public class SearchEngineMigrationHandler(
        IMigrationPlanExecutor migrationPlanExecutor,
        ICoreScopeProvider scopeProvider,
        IKeyValueService keyValueService)
        : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            var migrationPlan = new SearchEngineMigrationPlan();
            var upgrader = new Upgrader(migrationPlan);
            upgrader.ExecuteAsync(migrationPlanExecutor, scopeProvider, keyValueService).GetAwaiter().GetResult();
        }
    }
}
