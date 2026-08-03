using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Models;
using SearchEngineTools.Repositories;
using SearchEngineTools.Services;
using SearchEngineTools.Services.Providers;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace SearchEngineTools.Handlers
{
    /// <summary>
    /// Handles content published notifications to queue URLs for search engine submission.
    /// </summary>
    public class ContentPublishedHandler(
        ISearchEngineSubmissionQueueRepository queueRepository,
        IEnumerable<ISearchEngineSubmissionProvider> providers,
        IIndexNowSubmissionService indexNowSubmissionService,
        IUmbracoContextFactory umbracoContextFactory,
        IPublishedUrlProvider publishedUrlProvider,
        IOptions<ThrottlingOptions> throttlingOptions,
        ILogger<ContentPublishedHandler> logger
    ) : INotificationAsyncHandler<ContentPublishedNotification>
    {
        public async Task HandleAsync(ContentPublishedNotification notification, CancellationToken cancellationToken)
        {
            var excluded = new HashSet<string>(throttlingOptions.Value.ExcludedDocumentTypes, StringComparer.OrdinalIgnoreCase); //TODO: Implement the excluded document types.

            var throttling = throttlingOptions.Value;

            var submittedCount = 0;

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

                    var url = GetAbsoluteUrl(content);

                    if (string.IsNullOrWhiteSpace(url) || url == "#")
                    {
                        logger.LogDebug("Skipping Search Engine submission for content {ContentId} - {ContentName} because it has no valid URL",
                            content.Id,
                            content.Name);
                        continue;
                    }

                    try
                    {
                        var lastModified = content.UpdateDate.ToUniversalTime();
                        await queueRepository.UpsertPendingAsync(url, lastModified, cancellationToken);

                        if (submittedCount >= throttling.MaxBatchSize)
                        {
                            continue;
                        }

                        if (submittedCount > 0 && throttling.DelayBetweenSubmissionMs > 0)
                        {
                            await Task.Delay(throttling.DelayBetweenSubmissionMs, cancellationToken);
                        }

                        var anySuccess = false;
                        string? lastError = null;

                        foreach (var provider in providers.Where(p => p.IsEnabled))
                        {
                            var success = await provider.SubmitAsync(url, cancellationToken);
                            if (success)
                            {
                                anySuccess = true;
                            }
                            else
                            {
                                lastError = $"Submission failed for provider {provider.ProviderName}";
                                logger.LogWarning(lastError + " for URL {url}", url);
                            }
                        }

                        submittedCount++;

                        var status = anySuccess ? SearchEngineSubmissionStatus.Success : SearchEngineSubmissionStatus.Failed;

                        await queueRepository.UpdateSubmissionResultAsync(
                            url, status, anySuccess ? null : lastError, cancellationToken);

                        logger.LogInformation("Search Engine submission {status} for content {content.Id} - {content.Name} at {url}",
                           status,
                           content.Id,
                           content.Name,
                           url);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error submitting URL to search engine for content {content.Id} - {content.Name} at {url}",
                            content.Id,
                            content.Name,
                            url);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing Search Engine submission for content {content.Id} - {content.Name}",
                        content.Id,
                        content.Name);
                }
            }
        }

        private string? GetAbsoluteUrl(IContent content)
        {
            var contextRef = umbracoContextFactory.EnsureUmbracoContext();
            var publishedContent = contextRef.UmbracoContext.Content?.GetById(content.Id);

            if (publishedContent is null)
            {
                return null;
            }

            return publishedContent.Url(publishedUrlProvider, mode: UrlMode.Absolute);
        }
    }

}
