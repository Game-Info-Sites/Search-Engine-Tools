using System.Text.Json.Serialization;

namespace SearchEngineTools.Models
{
    public class SeoSettingsProperty
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("metaDescription")]
        public string? MetaDescription { get; set; }

        [JsonPropertyName("noIndexOption")]
        public bool NoIndexOption { get; set; }

        [JsonPropertyName("noFollowOption")]
        public bool NoFollowOption { get; set; }
    }
}
