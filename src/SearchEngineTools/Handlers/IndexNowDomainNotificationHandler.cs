using Microsoft.Extensions.Logging;
using SearchEngineTools.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace SearchEngineTools.Handlers
{
    public class IndexNowDomainNotificationHandler(
        IDomainService domainService,
        IIndexNowKeyService indexNowKeyService,
        ILogger<IndexNowDomainNotificationHandler> logger
    ) : INotificationAsyncHandler<UmbracoApplicationStartedNotification>,
        INotificationAsyncHandler<DomainSavedNotification>,
        INotificationAsyncHandler<DomainDeletedNotification>
    {
        public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
        {
            var domains = domainService.GetAll(true).ToList();

            if (domains.Count == 0)
            {
                logger.LogDebug("No Umbraco domains configured. IndexNow keys will be created lazily per request host.");
                return;
            }

            foreach (var domain in domains)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await indexNowKeyService.EnsureKeyForDomainAsync(domain.DomainName, cancellationToken);
            }
        }

        public async Task HandleAsync(DomainSavedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var domain in notification.SavedEntities)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await indexNowKeyService.EnsureKeyForDomainAsync(domain.DomainName, cancellationToken);
            }
        }

        public async Task HandleAsync(DomainDeletedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var domain in notification.DeletedEntities)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await indexNowKeyService.DeleteKeyForDomainAsync(domain.DomainName, cancellationToken);
            }
        }
    }
}
