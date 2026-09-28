using System.Text.Json;
using SearchEngineTools.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace SearchEngineTools.Converters
{
    public class SeoSettingsValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias == Constants.SeoSettings;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(SeoSettingsValue);

        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel cacheLevel, object? inter, bool preview)
        {
            if (inter is not string json || string.IsNullOrWhiteSpace(json)) { return null; }
            var stored = JsonSerializer.Deserialize<SeoSettingsProperty>(json);
            if (stored is null) { return null; }
            var title = stored.Title;
            var suffix = GetTitleSuffix(propertyType);

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(suffix)) { title = $"{title.TrimEnd()} {suffix.Trim()}"; }

            return new SeoSettingsValue
            {
                MetaDescription = stored.MetaDescription,
                NoFollowOption = stored.NoFollowOption,
                NoIndexOption = stored.NoIndexOption,
                Title = title
            };
        }

        private static string? GetTitleSuffix(IPublishedPropertyType propertyType)
        {
            if (propertyType.DataType.ConfigurationObject is IDictionary<string, object> config && config.TryGetValue("titleSuffix", out var suffix))
            {
                return suffix switch
                {
                    string s => s,
                    JsonElement { ValueKind: JsonValueKind.String } je => je.GetString(),
                    _ => suffix?.ToString()
                };
            }

            return null;
        }
    }
}
