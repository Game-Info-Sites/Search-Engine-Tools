namespace SearchEngineTools.Services.Providers
{
    /// <summary>
    /// Defines a search engine submission provider that can be used to submit URLs to search engines.
    /// Implement this interface to add support for additional search engines (Google, Bing...).
    /// Register your implementation via the <c>SearchEngineBuilderExtension.AddSearchEngineSubmissionProvider{TProvider}(this IUmbracoBuilder builder)</c> method.
    /// </summary>
    public interface ISearchEngineSubmissionProvider
    {
        public string ProviderName { get; }

        public bool IsEnabled { get; }

        public int MaxSubmissionPerDay { get; }

        public Task<bool> SubmitAsync(string url, CancellationToken cancellationToken);
    }
}
