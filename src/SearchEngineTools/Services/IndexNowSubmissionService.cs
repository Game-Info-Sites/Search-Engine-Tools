using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Services.Providers;

namespace SearchEngineTools.Services
{
    public class IndexNowSubmissionService(
        HttpClient httpClient,
        IOptions<SearchEngineToolsOptions> searchEngineToolsOptions,
        IOptions<IndexNowOptions> options,
        IIndexNowKeyService indexNowKeyService,
        ILogger<IndexNowSubmissionService> logger
    ) : IIndexNowSubmissionService, ISearchEngineSubmissionProvider
    {
        public string ProviderName => "IndexNow";

        public bool IsEnabled => searchEngineToolsOptions.Value.Enabled && options.Value.Enabled;

        public string? LastError { get; private set; }

        public int MaxSubmissionPerDay => options.Value.Throttling.MaxSubmissionsPerDay;

        public async Task<bool> SubmitAsync(string url, CancellationToken cancellationToken)
        {
            var configValue = options.Value;
            LastError = null;

            if (string.IsNullOrWhiteSpace(url))
            {
                LastError = "IndexNow submission skipped: URL is null or empty.";
                logger.LogWarning("{LastError}", LastError);
                return false;
            }

            if (!searchEngineToolsOptions.Value.Enabled)
            {
                LastError = "Search Engine Tools is disabled.";
                logger.LogDebug("IndexNow submission skipped for {Url}: {LastError}", url, LastError);
                return false;
            }

            if (!configValue.Enabled)
            {
                LastError = "IndexNow is disabled.";
                logger.LogDebug("IndexNow submission skipped for {Url}: {LastError}", url, LastError);
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                LastError = "URL is not absolute.";
                logger.LogWarning("IndexNow submission skipped for {Url}: {LastError}", url, LastError);
                return false;
            }

            var key = await indexNowKeyService.EnsureKeyForDomainAsync(uri.Host, cancellationToken);
            if (string.IsNullOrWhiteSpace(key))
            {
                LastError = $"No IndexNow key available for domain {uri.Host}.";
                logger.LogWarning("IndexNow submission skipped for {Url}: {LastError}", url, LastError);
                return false;
            }

            try
            {
                var keyLocation = string.IsNullOrWhiteSpace(configValue.KeyLocation)
                    ? BuildKeyLocation(uri, key)
                    : configValue.KeyLocation;
                var requestUrl = BuildRequestUrl(configValue.Endpoint, url, key, keyLocation);
                var response = await httpClient.GetAsync(requestUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("IndexNow submission successful for {url}", url);
                    return true;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    LastError = "IndexNow submission was rate limited. Will retry later.";
                    logger.LogWarning("{LastError} URL: {Url}", LastError, url);
                    return false;
                }

                LastError = $"IndexNow endpoint returned {(int)response.StatusCode} {response.ReasonPhrase}.";
                logger.LogWarning("IndexNow submission failed for {Url}: {LastError}", url, LastError);
                return false;
            }
            catch (HttpRequestException ex)
            {
                LastError = $"IndexNow HTTP request failed: {ex.Message}";
                logger.LogError(ex, "IndexNow HTTP request failed for {Url}", url);
                return false;
            }
            catch (TaskCanceledException ex)
            {
                LastError = "IndexNow request timed out.";
                logger.LogError(ex, "IndexNow request timed out for {Url}", url);
                return false;
            }
        }

        private static string BuildKeyLocation(Uri uri, string key)
        {
            return $"{uri.Scheme}://{uri.Host}/{key}.txt";
        }

        private static string BuildRequestUrl(string endpoint, string url, string key, string? keyLocation)
        {
            var requestUrl = $"{endpoint}?url={Uri.EscapeDataString(url)}&key={Uri.EscapeDataString(key)}";

            if (!string.IsNullOrWhiteSpace(keyLocation))
            {
                requestUrl += $"&keyLocation={Uri.EscapeDataString(keyLocation)}";
            }

            return requestUrl;
        }
    }
}
