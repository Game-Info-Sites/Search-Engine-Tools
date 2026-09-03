namespace SearchEngineTools.Models
{
    public class IndexNowKey
    {
        public const int MaxDomainLength = 255;

        public const int MaxKeyLength = 32;

        public int Id { get; set; }

        public string Domain { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; }
    }
}
