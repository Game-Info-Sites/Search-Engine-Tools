using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Models;
using SearchEngineTools.Repositories;
using SearchEngineTools.Services.Providers;

namespace SearchEngineTools.BackgroundServices
{
    public sealed class SearchEngineSubmissionQueueWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<SearchEngineToolsOptions> searchEngineToolsOptions,
        IOptions<ThrottlingOptions> throttlingOptions,
        ILogger<SearchEngineSubmissionQueueWorker> logger) : BackgroundService
    {
        private static readonly TimeSpan _pollInterval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Search Engine Tools Submission queue worker started");

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessQueueAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing Search Engine Tools submission queue");
                }

                try
                {
                    await Task.Delay(_pollInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            logger.LogInformation("SEO Submission queue worker stopped");
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            if (!searchEngineToolsOptions.Value.Enabled)
            {
                logger.LogDebug("Search Engine Tools is disabled. Skipping queue processing.");
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ISearchEngineSubmissionQueueRepository>();
            var providers = scope.ServiceProvider.GetServices<ISearchEngineSubmissionProvider>()
                .Where(provider => provider.IsEnabled)
                .ToList();

            if (providers.Count == 0)
            {
                logger.LogDebug("No enabled search engine submission providers configured. Skipping queue processing.");
                return;
            }

            var throttling = throttlingOptions.Value;
            var batchSize = Math.Max(1, throttling.MaxBatchSize);
            var pending = await repository.GetPendingAsync(batchSize, throttling.MaxRetryCount, cancellationToken);

            if (pending.Count == 0)
            {
                return;
            }


            var dailyCount = await repository.CountSuccessfulSubmissionsLast24HrsAsync(cancellationToken);

            var maxPerDay = providers.Where(provider => provider.MaxSubmissionPerDay > 0)
                .Select(provider => provider.MaxSubmissionPerDay)
                .DefaultIfEmpty(0)
                .Min();

            if (maxPerDay > 0 && dailyCount >= maxPerDay)
            {
                logger.LogInformation("Daily submission quota reached {DailyCount}/{MaxPerDay}. Skipping batch.", dailyCount, maxPerDay);
                return;
            }

            var remainingQuota = maxPerDay > 0 ? maxPerDay - dailyCount : int.MaxValue;
            var toProcess = pending.Take(remainingQuota).ToList();

            logger.LogInformation(
                "Processing {Count} pending submissions across {ProviderCount} provider(s)",
                toProcess.Count,
                providers.Count);

            for (var i = 0; i < toProcess.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var item = toProcess[i];

                var anySuccess = false;
                string? lastError = null;

                foreach (var provider in providers)
                {
                    var success = await provider.SubmitAsync(item.Url, cancellationToken);
                    if (success)
                    {
                        anySuccess = true;
                    }
                    else
                    {
                        lastError = string.IsNullOrWhiteSpace(provider.LastError)
                            ? $"{provider.ProviderName} submission failed."
                            : $"{provider.ProviderName}: {provider.LastError}";
                    }
                }

                if (!anySuccess && throttling.MaxRetryCount > 0 && item.RetryCount + 1 >= throttling.MaxRetryCount)
                {
                    lastError = providers.Count == 1
                        ? $"{providers[0].ProviderName} submission failed."
                        : "Search engine submission failed for all enabled providers.";
                }

                await repository.UpdateSubmissionResultAsync(
                    item.Url,
                    anySuccess ? SearchEngineSubmissionStatus.Success : SearchEngineSubmissionStatus.Failed,
                    anySuccess ? null : lastError,
                    cancellationToken);

                // Delay between submissions
                if (i < toProcess.Count - 1 && throttling.DelayBetweenSubmissionMs > 0)
                {
                    await Task.Delay(throttling.DelayBetweenSubmissionMs, cancellationToken);
                }
            }
        }
    }
}
