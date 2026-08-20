using System.Text.Json.Serialization;

namespace SearchEngineTools.Models
{
    //Property Value model for use to access the properties' values from a cshtml template.
    public class ContentDatesValue
    {
        [JsonPropertyName("publishedOn")]
        public DateTime? PublishedOn { get; set; }

        [JsonPropertyName("lastSignificantUpdate")]
        public DateTime? LastSignificantUpdate { get; set; }
    }
}
