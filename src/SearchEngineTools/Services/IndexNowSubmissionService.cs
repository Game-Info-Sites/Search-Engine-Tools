using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchEngineTools.Configuration;
using SearchEngineTools.Services.Providers;

namespace SearchEngineTools.Services
{
    public class IndexNowSubmissionService(
        HttpClient httpClient,
        IOptions<IndexNowOptions> options,
        IIndexNowKeyService indexNowKeyService,
        ILogger<IndexNowSubmissionService> logger
    ) : IIndexNowSubmissionService, ISearchEngineSubmissionProvider
    {
        public string ProviderName => "IndexNow";

        public bool IsEnabled => options.Value.Enabled;

        public int MaxSubmissionPerDay => options.Value.Throttling.MaxSubmissionsPerDay;

        public async Task<bool> SubmitAsync(string url, CancellationToken cancellationToken)
        {
            var configValue = options.Value;

            if (string.IsNullOrWhiteSpace(url))
            {
                logger.LogWarning("IndexNow submission skipped: URl is null or empty");
                return false;
            }

            if (!configValue.Enabled)
            {
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                logger.LogWarning("IndexNow submission skipped for {Url}: URL is not absolute", url);
                return false;
            }

            var key = await indexNowKeyService.EnsureKeyForDomainAsync(uri.Host, cancellationToken);
            if (string.IsNullOrWhiteSpace(key))
            {
                logger.LogWarning("IndexNow submission skipped for {Url}: No key available for domain {Domain}", url, uri.Host);
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
                    logger.LogWarning("IndexNow submission rate limited for {url}. Will retry later.", url);
                    return false;
                }

                return false;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "IndexNow HTTP request failed for {Url}", url);
                return false;
            }
            catch (TaskCanceledException ex)
            {
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
