using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SearchEngineTools.Handlers
{
    /// <summary>
    /// Handles content published notifications to queue URLs for search engine submission.
    /// </summary>
    public class ContentPublishedHandler(
        IContentUrlResolver contentUrlResolver,
        ISearchEngineUrlSubmissionService submissionService,
        IOptions<SearchEngineToolsOptions> searchEngineToolsOptions,
        IOptions<ThrottlingOptions> throttlingOptions,
        ILogger<ContentPublishedHandler> logger
    ) : INotificationAsyncHandler<ContentPublishedNotification>
    {
        public async Task HandleAsync(ContentPublishedNotification notification, CancellationToken cancellationToken)
        {
            if (!searchEngineToolsOptions.Value.Enabled)
            {
                logger.LogInformation("Search Engine Tools is disabled. Skipping published content URL submission.");
                return;
            }

            var excluded = new HashSet<string>(throttlingOptions.Value.ExcludedDocumentTypes, StringComparer.OrdinalIgnoreCase); //TODO: Implement the excluded document types.

            foreach (var content in notification.PublishedEntities)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (excluded.Contains(content.ContentType.Alias))
                    {
                        logger.LogInformation("Skipping Search Engine submission for content {ContentId} - {ContentName} due to excluded document type {ContentTypeAlias}",
                            content.Id,
                            content.Name,
                            content.ContentType.Alias);
                        continue;
                    }

                    var url = contentUrlResolver.GetAbsoluteUrl(content);

                    if (string.IsNullOrWhiteSpace(url) || url == "#")
                    {
                        logger.LogDebug("Skipping Search Engine submission for content {ContentId} - {ContentName} because it has no valid URL",
                            content.Id,
                            content.Name);
                        continue;
                    }

                    await submissionService.SubmitAsync(url, content.UpdateDate.ToUniversalTime(), cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing Search Engine submission for content {content.Id} - {content.Name}",
                        content.Id,
                        content.Name);
                }
            }
        }
    }

}
