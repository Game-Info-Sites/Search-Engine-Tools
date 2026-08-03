namespace SearchEngineTools.Services
{
    public interface IIndexNowSubmissionService
    {
        /// <summary>
        /// Submit a URL to the IndexNow API for immediate indexing
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> SubmitAsync(string url, CancellationToken cancellationToken);
    }
}
