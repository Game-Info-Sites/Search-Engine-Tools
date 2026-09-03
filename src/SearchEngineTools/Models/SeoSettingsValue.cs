using System.Text.Json.Serialization;

namespace SearchEngineTools.Models
{
    //Property Value model for use to access the properties' values from a cshtml template.
    public class SeoSettingsValue
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
