using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SearchEngineTools.Data;
using SearchEngineTools.Models;

namespace SearchEngineTools.Services
{
    public class IndexNowKeyService(
        SearchEngineToolsDbContext dbContext,
        ILogger<IndexNowKeyService> logger
    ) : IIndexNowKeyService
    {
        public async Task<string?> GetKeyForDomainAsync(string domain, CancellationToken cancellation = default)
        {
            var normalizedDomain = NormalizeDomain(domain);
            if (normalizedDomain is null)
            {
                return null;
            }

            return await dbContext.IndexNowKeys
                .Where(x => x.Domain == normalizedDomain)
                .Select(x => x.Key)
                .FirstOrDefaultAsync(cancellation);
        }

        public async Task<string?> EnsureKeyForDomainAsync(string domain, CancellationToken cancellation = default)
        {
            var normalizedDomain = NormalizeDomain(domain);
            if (normalizedDomain is null)
            {
                logger.LogDebug("IndexNow key was not created because domain {Domain} could not be normalized", domain);
                return null;
            }

            var existingKey = await GetKeyForDomainAsync(normalizedDomain, cancellation);
            if (!string.IsNullOrWhiteSpace(existingKey))
            {
                return existingKey;
            }

            var indexNowKey = new IndexNowKey
            {
                Domain = normalizedDomain,
                Key = GenerateKey(),
                CreatedUtc = DateTime.UtcNow,
            };

            dbContext.IndexNowKeys.Add(indexNowKey);

            try
            {
                await dbContext.SaveChangesAsync(cancellation);
                logger.LogInformation("Created IndexNow key for domain {Domain}", normalizedDomain);
                return indexNowKey.Key;
            }
            catch (DbUpdateException)
            {
                dbContext.Entry(indexNowKey).State = EntityState.Detached;

                existingKey = await GetKeyForDomainAsync(normalizedDomain, cancellation);
                if (!string.IsNullOrWhiteSpace(existingKey))
                {
                    return existingKey;
                }

                throw;
            }
        }

        public async Task DeleteKeyForDomainAsync(string domain, CancellationToken cancellation = default)
        {
            var normalizedDomain = NormalizeDomain(domain);
            if (normalizedDomain is null)
            {
                return;
            }

            var indexNowKey = await dbContext.IndexNowKeys
                .FirstOrDefaultAsync(x => x.Domain == normalizedDomain, cancellation);

            if (indexNowKey is null)
            {
                return;
            }

            dbContext.IndexNowKeys.Remove(indexNowKey);
            await dbContext.SaveChangesAsync(cancellation);
            logger.LogInformation("Deleted IndexNow key for domain {Domain}", normalizedDomain);
        }

        public async Task<bool> KeyBelongsToDomainAsync(string domain, string key, CancellationToken cancellation = default)
        {
            var normalizedDomain = NormalizeDomain(domain);
            var normalizedKey = key.Trim().ToLowerInvariant();
            if (normalizedDomain is null || normalizedKey.Length != IndexNowKey.MaxKeyLength)
            {
                return false;
            }

            return await dbContext.IndexNowKeys
                .AnyAsync(
                    x => x.Domain == normalizedDomain && x.Key == normalizedKey,
                    cancellation);
        }

        private static string GenerateKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static string? NormalizeDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return null;
            }

            var value = domain.Trim().TrimEnd('/');

            if (value == "*")
            {
                return null;
            }

            if (value.Contains("://", StringComparison.Ordinal) && Uri.TryCreate(value, UriKind.Absolute, out var absoluteUri))
            {
                return NormalizeHost(absoluteUri.Host);
            }

            var pathIndex = value.IndexOfAny(['/', '?', '#']);
            if (pathIndex >= 0)
            {
                value = value[..pathIndex];
            }

            if (Uri.TryCreate($"http://{value}", UriKind.Absolute, out var hostUri) && !string.IsNullOrWhiteSpace(hostUri.Host))
            {
                return NormalizeHost(hostUri.Host);
            }

            return NormalizeHost(value);
        }

        private static string? NormalizeHost(string host)
        {
            var normalizedHost = host.Trim().TrimEnd('.').ToLowerInvariant();

            if (normalizedHost.Count(x => x == ':') == 1)
            {
                normalizedHost = normalizedHost[..normalizedHost.IndexOf(':')];
            }

            return string.IsNullOrWhiteSpace(normalizedHost) || normalizedHost.Length > IndexNowKey.MaxDomainLength
                ? null
                : normalizedHost;
        }
    }

}
