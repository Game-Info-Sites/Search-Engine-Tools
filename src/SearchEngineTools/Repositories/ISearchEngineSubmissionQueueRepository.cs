using SearchEngineTools.Models;

namespace SearchEngineTools.Repositories
{
    /// <summary>
    /// Persistence contract for the Search Engine submission queue.
    /// </summary>
    public interface ISearchEngineSubmissionQueueRepository
    {

        /// <summary>
        /// Insert a new pending entry
        /// </summary>
        /// <param name="url"></param>
        /// <param name="lastModifiedUtc"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Task UpsertPendingAsync(string url, DateTime lastModifiedUtc, CancellationToken cancellation = default);

        /// <summary>
        /// Update the outcome of a submission attempt.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="status"></param>
        /// <param name="error"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Task UpdateSubmissionResultAsync(string url, SearchEngineSubmissionStatus status, string? error = null, CancellationToken cancellation = default);

        /// <summary>
        /// Retrieves a list of pending Search Engine submission items, up to the specified maximum number of items.
        /// </summary>
        /// <param name="maxItems">The maximum number of items to retrieve. Must be a positive integer.</param>
        /// <param name="cancellation">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="SearchEngineSubmissionItem"/> objects representing the pending submissions. The list will be empty if no items are
        /// pending.</returns>
        public Task<IReadOnlyList<SearchEngineSubmissionItem>> GetPendingAsync(int maxItems, CancellationToken cancellation = default);

        /// <summary>
        /// Returns the count of successful submissions within the last 24 hours. This is used for throttling purposes to ensure we do not exceed daily submission limits set by search engine providers.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Task<int> CountSuccessfulSubmissionsLast24HrsAsync(CancellationToken cancellation = default);
    }
}
