namespace SearchEngineTools.Configuration
{
    /// <summary>
    /// Top-level configuration options for Search Engine Tools.
    /// </summary>
    public class SearchEngineToolsOptions
    {
        public const string SectionName = "SearchEngineTools";

        /// <summary>
        /// Gets or sets a value indicating whether Search Engine Tools is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
