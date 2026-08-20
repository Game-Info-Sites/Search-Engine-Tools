namespace SearchEngineTools.Services
{
    /// <summary>
    /// Temporarily tracks content URLs captured before a URL-changing operation completes.
    /// </summary>
    public interface IContentUrlChangeTracker
    {
        /// <summary>
        /// Captures URLs for a content operation.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="contentKey">The content key.</param>
        /// <param name="urls">The URLs to capture.</param>
        public void Capture(string operation, Guid contentKey, IEnumerable<string> urls);

        /// <summary>
        /// Retrieves and removes captured URLs for a content operation.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="contentKey">The content key.</param>
        /// <returns>The captured URLs, or an empty collection when none were captured.</returns>
        public IReadOnlyCollection<string> Pop(string operation, Guid contentKey);
    }
}
