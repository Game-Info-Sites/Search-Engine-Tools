namespace SearchEngineTools.Services
{
    public interface IIndexNowKeyService
    {
        public Task<string?> GetKeyForDomainAsync(string domain, CancellationToken cancellation = default);

        public Task<string?> EnsureKeyForDomainAsync(string domain, CancellationToken cancellation = default);

        public Task DeleteKeyForDomainAsync(string domain, CancellationToken cancellation = default);

        public Task<bool> KeyBelongsToDomainAsync(string domain, string key, CancellationToken cancellation = default);
    }
}
