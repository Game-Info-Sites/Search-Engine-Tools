namespace SearchEngineTools.Services
{
    /// <summary>
    /// Provides feature enablement state for Search Engine Tools.
    /// </summary>
    public interface ISearchEngineToolsFeatureService
    {
        /// <summary>
        /// Gets a value indicating whether Search Engine Tools is enabled.
        /// </summary>
        public bool IsSearchEngineToolsEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether IndexNow is enabled.
        /// </summary>
        public bool IsIndexNowEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether URL submission is enabled for at least one provider.
        /// </summary>
        public bool IsUrlSubmissionEnabled { get; }
    }
}
