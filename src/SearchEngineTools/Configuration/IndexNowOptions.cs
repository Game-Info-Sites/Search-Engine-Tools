namespace SearchEngineTools.Configuration
{
    /// <summary>
    /// Configuration options for the IndexNow submission provider.
    /// </summary>
    public class IndexNowOptions
    {
        public const string SectionName = "SearchEngineTools:IndexNow";

        public bool Enabled { get; set; } = true;

        public string? DefaultKey { get; set; }

        public string Endpoint { get; set; } = "https://api.indexnow.org/indexnow";

        public string? KeyLocation { get; set; }

        public Dictionary<string, string> DomainKeys { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public ThrottlingOptions Throttling { get; set; } = new();

        /// <summary>
        /// Return the IndexNow API Key to use for <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string? GetKeyForUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return DefaultKey;
            }

            if (DomainKeys.TryGetValue(uri.Host, out var key))
            {
                return key;
            }
            return DefaultKey;
        }
    }
}
