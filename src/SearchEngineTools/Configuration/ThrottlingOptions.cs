namespace SearchEngineTools.Configuration
{
    /// <summary>
    /// Provider-agnostic throttling options for search engine submission providers.
    /// </summary>
    public class ThrottlingOptions
    {
        public const string SectionName = "SearchEngineTools:Throttling";

        /// <summary>
        /// Gets or sets the maximum number of submissions per rolling 24-hour period
        /// Set to 0 for unlimited.
        /// </summary>
        public int MaxSubmissionsPerDay { get; set; } = 100;

        /// <summary>
        /// Gets or sets the delay in milliseconds between individual submissions
        /// </summary>
        public int DelayBetweenSubmissionMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the maximum number of retry attempts
        /// </summary>
        public int MaxRetryCount { get; set; } = 5;

        /// <summary>
        /// Gets or sets the maximum number of URLs to submit in a single batch
        /// </summary>
        public int MaxBatchSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets the list of document type aliases excluded from Seo submission
        /// </summary>
        public List<string> ExcludedDocumentTypes { get; set; } = new();

    }
}
