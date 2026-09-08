namespace SearchEngineTools.Models
{
    public class SearchEngineSubmissionItem
    {
        public const int MaxErrorLength = 4000;

        public const int MaxUrlLength = 2050;

        public int Id { get; set; }

        public string Url { get; set; } = string.Empty;

        public SearchEngineSubmissionStatus Status { get; set; }

        public DateTime LastModifiedUtc { get; set; }

        public DateTime? LastSubmittedUtc { get; set; }

        public DateTime? LastAttemptUtc { get; set; }

        public int RetryCount { get; set; }

        public string? LastError { get; set; }
    }
}
