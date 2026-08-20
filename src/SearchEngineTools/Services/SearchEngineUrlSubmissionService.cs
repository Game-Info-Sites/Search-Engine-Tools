using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Models;
using SearchEngineTools.Repositories;
using SearchEngineTools.Services.Providers;

namespace SearchEngineTools.Services
{
    public class SearchEngineUrlSubmissionService(
        ISearchEngineSubmissionQueueRepository queueRepository,
        IEnumerable<ISearchEngineSubmissionProvider> providers,
        IOptions<SearchEngineToolsOptions> searchEngineToolsOptions,
        IOptions<ThrottlingOptions> throttlingOptions,
        ILogger<SearchEngineUrlSubmissionService> logger
    ) : ISearchEngineUrlSubmissionService
    {
        public async Task SubmitAsync(string? url, DateTime lastModifiedUtc, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(url) || url == "#")
            {
                return;
            }

            await SubmitAsync([url], lastModifiedUtc, cancellationToken);
        }

        public async Task SubmitAsync(IEnumerable<string> urls, DateTime lastModifiedUtc, CancellationToken cancellationToken = default)
        {
            if (!searchEngineToolsOptions.Value.Enabled)
            {
                logger.LogDebug("Search Engine Tools is disabled. Skipping URL submission.");
                return;
            }

            var distinctUrls = urls
                .Where(IsValidUrl)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (distinctUrls.Length == 0)
            {
                return;
            }

            foreach (var url in distinctUrls)
            {
                await queueRepository.UpsertPendingAsync(url, lastModifiedUtc, cancellationToken);
            }

            var enabledProviders = providers.Where(p => p.IsEnabled).ToArray();
            if (enabledProviders.Length == 0)
            {
                logger.LogInformation("Queued {Count} URL(s), but no enabled search engine submission providers are configured.", distinctUrls.Length);
                foreach (var url in distinctUrls)
                {
                    await queueRepository.UpdateSubmissionResultAsync(
                        url,
                        SearchEngineSubmissionStatus.Pending,
                        "Submission skipped: no enabled search engine submission providers are configured.",
                        cancellationToken);
                }

                return;
            }

            var throttling = throttlingOptions.Value;
            var submittedCount = 0;

            foreach (var url in distinctUrls)
            {
                cancellationToken.ThrowIfCancellationRequested();

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

                foreach (var provider in enabledProviders)
                {
                    var success = await provider.SubmitAsync(url, cancellationToken);
                    if (success)
                    {
                        anySuccess = true;
                    }
                    else
                    {
                        lastError = string.IsNullOrWhiteSpace(provider.LastError)
                            ? $"Submission failed for provider {provider.ProviderName}."
                            : $"{provider.ProviderName}: {provider.LastError}";
                        logger.LogWarning("{LastError} for URL {Url}", lastError, url);
                    }
                }

                submittedCount++;

                await queueRepository.UpdateSubmissionResultAsync(
                    url,
                    anySuccess ? SearchEngineSubmissionStatus.Success : SearchEngineSubmissionStatus.Failed,
                    anySuccess ? null : lastError,
                    cancellationToken);

                logger.LogInformation("Search Engine submission {Status} for URL {Url}", anySuccess ? SearchEngineSubmissionStatus.Success : SearchEngineSubmissionStatus.Failed, url);
            }
        }

        private static bool IsValidUrl(string? url)
        {
            return !string.IsNullOrWhiteSpace(url) && url != "#";
        }
    }
}
