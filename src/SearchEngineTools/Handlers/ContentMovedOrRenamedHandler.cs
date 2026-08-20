using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace SearchEngineTools.Handlers
{
    public class ContentMovedOrRenamedHandler(
        IContentUrlResolver contentUrlResolver,
        IContentUrlChangeTracker contentUrlChangeTracker,
        ISearchEngineUrlSubmissionService submissionService,
        IOptions<SearchEngineToolsOptions> searchEngineToolsOptions,
        ILogger<ContentMovedOrRenamedHandler> logger
    ) : INotificationHandler<ContentMovingNotification>,
        INotificationAsyncHandler<ContentMovedNotification>,
        INotificationHandler<RenamingNotification<IContent>>,
        INotificationAsyncHandler<RenamedNotification<IContent>>
    {
        private const string MoveOperation = "move";
        private const string RenameOperation = "rename";

        public void Handle(ContentMovingNotification notification)
        {
            CaptureDescendantUrls(MoveOperation, notification.MoveInfoCollection.Select(x => x.Entity));
        }

        public async Task HandleAsync(ContentMovedNotification notification, CancellationToken cancellationToken)
        {
            await SubmitOldAndNewDescendantUrlsAsync(MoveOperation, notification.MoveInfoCollection.Select(x => x.Entity), cancellationToken);
        }

        public void Handle(RenamingNotification<IContent> notification)
        {
            CaptureDescendantUrls(RenameOperation, notification.Entities);
        }

        public async Task HandleAsync(RenamedNotification<IContent> notification, CancellationToken cancellationToken)
        {
            await SubmitOldAndNewDescendantUrlsAsync(RenameOperation, notification.Entities, cancellationToken);
        }

        private void CaptureDescendantUrls(string operation, IEnumerable<IContent> entities)
        {
            if (!searchEngineToolsOptions.Value.Enabled)
            {
                return;
            }

            foreach (var content in entities)
            {
                var urls = contentUrlResolver.GetAbsoluteUrlsForDescendantsAndSelf(content);
                contentUrlChangeTracker.Capture(operation, content.Key, urls);
            }
        }

        private async Task SubmitOldAndNewDescendantUrlsAsync(string operation, IEnumerable<IContent> entities, CancellationToken cancellationToken)
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

                    var oldUrls = contentUrlChangeTracker.Pop(operation, content.Key);
                    var newUrls = contentUrlResolver.GetAbsoluteUrlsForDescendantsAndSelf(content);
                    var urls = oldUrls
                        .Concat(newUrls)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    if (urls.Length == 0)
                    {
                        logger.LogDebug("No Search Engine URLs found for {Operation} content {ContentId} - {ContentName}", operation, content.Id, content.Name);
                        continue;
                    }

                    await submissionService.SubmitAsync(urls, DateTime.UtcNow, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error submitting Search Engine URLs for {Operation} content {ContentId} - {ContentName}", operation, content.Id, content.Name);
                }
            }
        }
    }
}
