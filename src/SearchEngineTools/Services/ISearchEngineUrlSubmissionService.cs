namespace SearchEngineTools.Services
{
    /// <summary>
    /// Queues and submits URLs through enabled search engine submission providers.
    /// </summary>
    public interface ISearchEngineUrlSubmissionService
    {
        /// <summary>
        /// Queues and submits a URL.
        /// </summary>
        /// <param name="url">The absolute URL to submit.</param>
        /// <param name="lastModifiedUtc">The UTC date and time the URL was last modified.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task SubmitAsync(string? url, DateTime lastModifiedUtc, CancellationToken cancellationToken = default);

        /// <summary>
        /// Queues and submits URLs.
        /// </summary>
        /// <param name="urls">The absolute URLs to submit.</param>
        /// <param name="lastModifiedUtc">The UTC date and time the URLs were last modified.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task SubmitAsync(IEnumerable<string> urls, DateTime lastModifiedUtc, CancellationToken cancellationToken = default);
    }
}
