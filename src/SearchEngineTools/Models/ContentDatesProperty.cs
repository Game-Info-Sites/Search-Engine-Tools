using System.Text.Json.Serialization;

namespace SearchEngineTools.Models
{
    public class ContentDatesProperty
    {
        [JsonPropertyName("publishedOn")]
        public DateTime? PublishedOn { get; set; }

        [JsonPropertyName("lastSignificantUpdate")]
        public DateTime? LastSignificantUpdate { get; set; }

        [JsonPropertyName("isSignificantUpdate")]
        public bool IsSignificantUpdate { get; set; }
    }
}
