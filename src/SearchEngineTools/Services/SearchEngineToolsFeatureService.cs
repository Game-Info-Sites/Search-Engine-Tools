using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;

namespace SearchEngineTools.Services
{
    public class SearchEngineToolsFeatureService(
        IOptions<SearchEngineToolsOptions> searchEngineToolsOptions,
        IOptions<IndexNowOptions> indexNowOptions
    ) : ISearchEngineToolsFeatureService
    {
        public bool IsSearchEngineToolsEnabled => searchEngineToolsOptions.Value.Enabled;

        public bool IsIndexNowEnabled => IsSearchEngineToolsEnabled && indexNowOptions.Value.Enabled;

        public bool IsUrlSubmissionEnabled => IsIndexNowEnabled;
    }
}
