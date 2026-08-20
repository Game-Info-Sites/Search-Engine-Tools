using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace SearchEngineTools.Handlers
{
    public class ContentDeletedOrUnpublishedHandler(
        IContentUrlResolver contentUrlResolver,
        IContentUrlChangeTracker contentUrlChangeTracker,
        ISearchEngineUrlSubmissionService submissionService,
        IOptions<SearchEngineToolsOptions> searchEngineToolsOptions,
        ILogger<ContentDeletedOrUnpublishedHandler> logger
    ) : INotificationHandler<ContentDeletingNotification>,
        INotificationAsyncHandler<ContentDeletedNotification>,
        INotificationHandler<ContentUnpublishingNotification>,
        INotificationAsyncHandler<ContentUnpublishedNotification>
    {
        private const string DeleteOperation = "delete";
        private const string UnpublishOperation = "unpublish";

        public void Handle(ContentDeletingNotification notification)
        {
            CaptureUrls(DeleteOperation, notification.DeletedEntities);
        }

        public async Task HandleAsync(ContentDeletedNotification notification, CancellationToken cancellationToken)
        {
            await SubmitCapturedUrlsAsync(DeleteOperation, notification.DeletedEntities, cancellationToken);
        }

        public void Handle(ContentUnpublishingNotification notification)
        {
            CaptureUrls(UnpublishOperation, notification.UnpublishedEntities);
        }

        public async Task HandleAsync(ContentUnpublishedNotification notification, CancellationToken cancellationToken)
        {
            await SubmitCapturedUrlsAsync(UnpublishOperation, notification.UnpublishedEntities, cancellationToken);
        }

        private void CaptureUrls(string operation, IEnumerable<IContent> entities)
        {
            if (!searchEngineToolsOptions.Value.Enabled)
            {
                return;
            }

            foreach (var content in entities)
            {
                var url = contentUrlResolver.GetAbsoluteUrl(content);
                if (string.IsNullOrWhiteSpace(url) || url == "#")
                {
                    continue;
                }

                contentUrlChangeTracker.Capture(operation, content.Key, [url]);
            }
        }

        private async Task SubmitCapturedUrlsAsync(string operation, IEnumerable<IContent> entities, CancellationToken cancellationToken)
        {
            if (!searchEngineToolsOptions.Value.Enabled)
            {
                return;
            }

            foreach (var content in entities)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var urls = contentUrlChangeTracker.Pop(operation, content.Key);
                    if (urls.Count == 0)
                    {
                        logger.LogDebug("No captured Search Engine URL found for {Operation} content {ContentId} - {ContentName}", operation, content.Id, content.Name);
                        continue;
                    }

                    await submissionService.SubmitAsync(urls, DateTime.UtcNow, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error submitting Search Engine URL for {Operation} content {ContentId} - {ContentName}", operation, content.Id, content.Name);
                }
            }
        }
    }
}
