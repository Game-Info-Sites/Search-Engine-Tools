using System.Collections.Concurrent;

namespace SearchEngineTools.Services
{
    public class ContentUrlChangeTracker : IContentUrlChangeTracker
    {
        private readonly ConcurrentDictionary<string, IReadOnlyCollection<string>> _capturedUrls = new(StringComparer.OrdinalIgnoreCase);

        public void Capture(string operation, Guid contentKey, IEnumerable<string> urls)
        {
            var captured = urls
                .Where(IsValidUrl)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (captured.Length == 0)
            {
                return;
            }

            _capturedUrls[BuildKey(operation, contentKey)] = captured;
        }

        public IReadOnlyCollection<string> Pop(string operation, Guid contentKey)
        {
            return _capturedUrls.TryRemove(BuildKey(operation, contentKey), out var urls)
                ? urls
                : Array.Empty<string>();
        }

        private static string BuildKey(string operation, Guid contentKey)
        {
            return $"{operation}:{contentKey:N}";
        }

        private static bool IsValidUrl(string? url)
        {
            return !string.IsNullOrWhiteSpace(url) && url != "#";
        }
    }
}
