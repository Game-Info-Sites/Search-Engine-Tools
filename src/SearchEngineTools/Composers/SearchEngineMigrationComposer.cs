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
            builder.AddNotificationHandler<ContentDeletingNotification, ContentDeletedOrUnpublishedHandler>();
            builder.AddNotificationAsyncHandler<ContentDeletedNotification, ContentDeletedOrUnpublishedHandler>();
            builder.AddNotificationHandler<ContentUnpublishingNotification, ContentDeletedOrUnpublishedHandler>();
            builder.AddNotificationAsyncHandler<ContentUnpublishedNotification, ContentDeletedOrUnpublishedHandler>();
            builder.AddNotificationHandler<ContentMovingNotification, ContentMovedOrRenamedHandler>();
            builder.AddNotificationAsyncHandler<ContentMovedNotification, ContentMovedOrRenamedHandler>();
            builder.AddNotificationHandler<RenamingNotification<Umbraco.Cms.Core.Models.IContent>, ContentMovedOrRenamedHandler>();
            builder.AddNotificationAsyncHandler<RenamedNotification<Umbraco.Cms.Core.Models.IContent>, ContentMovedOrRenamedHandler>();
            builder.AddNotificationAsyncHandler<DomainSavedNotification, IndexNowDomainNotificationHandler>();
            builder.AddNotificationAsyncHandler<DomainDeletedNotification, IndexNowDomainNotificationHandler>();

            builder.Services.Configure<SearchEngineToolsOptions>(
                builder.Config.GetSection(SearchEngineToolsOptions.SectionName));

            builder.Services.Configure<IndexNowOptions>(
                builder.Config.GetSection(IndexNowOptions.SectionName));

            builder.Services.Configure<ThrottlingOptions>(
                builder.Config.GetSection(ThrottlingOptions.SectionName));

            builder.Services.AddDbContext<SearchEngineToolsDbContext>((serviceProvider, options) =>
            {
                var connectionString =
                    builder.Config.GetConnectionString("umbracoDbDSN");

                var webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var dataDirectory = Path.Combine(webHostEnvironment.ContentRootPath, "umbraco", "Data");

                Directory.CreateDirectory(dataDirectory);

                connectionString = connectionString.Replace("|DataDirectory|", dataDirectory);

                options.UseSqlite(connectionString);
            });

            builder.Services.AddScoped<ISearchEngineSubmissionQueueRepository, SearchEngineSubmissionQueueRepository>();
            builder.Services.AddSingleton<IContentUrlChangeTracker, ContentUrlChangeTracker>();
            builder.Services.AddScoped<IContentUrlResolver, ContentUrlResolver>();
            builder.Services.AddScoped<ISearchEngineUrlSubmissionService, SearchEngineUrlSubmissionService>();
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
