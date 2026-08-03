using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SearchEngineTools.Data;
using SearchEngineTools.Models;

namespace SearchEngineTools.Repositories
{
    public class SearchEngineSubmissionQueueRepository(
    SearchEngineToolsDbContext dbContext,
    ILogger<SearchEngineSubmissionQueueRepository> logger
) : ISearchEngineSubmissionQueueRepository
    {
        private const int MaxErrorLength = 4000;

        /// <inheritdoc />
        public async Task UpsertPendingAsync(string url, DateTime lastModifiedUtc, CancellationToken cancellation)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url);

            var item = await dbContext.SearchEngineSubmissionQueue
                        .FirstOrDefaultAsync(
                            x => x.Url == url,
                            cancellation);

            if (item is null)
            {
                item = new SearchEngineSubmissionItem
                {
                    Url = url,
                    LastModifiedUtc = lastModifiedUtc,
                    Status = SearchEngineSubmissionStatus.Pending,
                    RetryCount = 0,
                };
                dbContext.SearchEngineSubmissionQueue.Add(item);
                await dbContext.SaveChangesAsync(cancellation);
                logger.LogDebug("Queued new URL for submission: {url}", url);
            }
            else
            {
                item.LastModifiedUtc = lastModifiedUtc;
                item.Status = SearchEngineSubmissionStatus.Pending;
                item.LastError = null;
                dbContext.SearchEngineSubmissionQueue.Update(item);
                await dbContext.SaveChangesAsync(cancellation);
                logger.LogDebug("Updated existing queue entry for URL: {url}", url);
            }
        }

        /// <inheritdoc />
        public async Task UpdateSubmissionResultAsync(string url, SearchEngineSubmissionStatus status, string? error, CancellationToken cancellation)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url);

            var item = await dbContext.SearchEngineSubmissionQueue
                        .FirstOrDefaultAsync(
                            x => x.Url == url,
                            cancellation);

            if (item is null)
            {
                logger.LogWarning("Cannot update submission result: {url} not found in queue", url);
                return;
            }

            var nowDate = DateTime.UtcNow;
            item.Status = status;
            item.LastAttemptUtc = nowDate;

            if (status == SearchEngineSubmissionStatus.Success)
            {
                item.LastSubmittedUtc = nowDate;
                item.LastError = null;
                item.RetryCount = 0;
            }
            else
            {
                item.RetryCount++;
                item.LastError = Truncate(error ?? "Submission failed.", MaxErrorLength);
            }

            dbContext.SearchEngineSubmissionQueue.Update(item);
            await dbContext.SaveChangesAsync(cancellation);
        }

        private string? Truncate(string v, int maxLength)
        {
            return v.Length <= maxLength ? v : v[..maxLength];
        }

        /// <inheritdoc />
        public async Task<int> CountSuccessfulSubmissionsLast24HrsAsync(CancellationToken cancellation = default)
        {

            var oneDayBefore = DateTime.UtcNow.AddHours(-24);

            return await dbContext.SearchEngineSubmissionQueue
                .CountAsync(
                    x =>
                        x.Status == SearchEngineSubmissionStatus.Success &&
                        x.LastSubmittedUtc >= oneDayBefore,
                    cancellation);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<SearchEngineSubmissionItem>> GetPendingAsync(int maxItems, CancellationToken cancellation = default)
        {
            if (maxItems <= 0)
            {
                return Array.Empty<SearchEngineSubmissionItem>();
            }

            return await dbContext.SearchEngineSubmissionQueue
                .Where(x =>
                    x.Status == SearchEngineSubmissionStatus.Pending ||
                    x.Status == SearchEngineSubmissionStatus.Failed)
                .OrderBy(x => x.LastModifiedUtc)
                .Take(maxItems)
                .ToListAsync(cancellation);
        }
    }

}
